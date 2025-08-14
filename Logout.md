Since you're using **EF Core** for ASP.NET Core Identity in your `ESLAdmin` project with Firebird, FastEndpoints, JWT authentication, and refresh tokens, the logout function can be implemented by revoking refresh tokens stored in a `RefreshTokens` table. This aligns with your existing architecture, which includes `AuthenticationRepository`, `DbContextDapper` for Dapper operations, `IConfigurationParams`, `IMessageLogger`, and the `ErrorOr` pattern. The logout process will invalidate refresh tokens for the authenticated user, ensuring they can no longer refresh their JWT access token. Since JWT access tokens are stateless, we'll focus on revoking refresh tokens (optionally, you can add a blacklist for access tokens if needed).

Below is a step-by-step procedure to implement the logout function, integrating with EF Core Identity and your existing patterns.

### Step-by-Step Procedure to Implement Logout with EF Core Identity

#### Step 1: Define the RefreshTokens Entity
Ensure you have a `RefreshToken` entity in your EF Core `IdentityDbContext` to store refresh tokens. If not already present, add it to your Firebird database and EF Core model.

In `ESLAdmin.Infrastructure.Data/ApplicationDbContext.cs`:

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ESLAdmin.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Token).IsRequired().HasMaxLength(512);
            entity.Property(e => e.ExpiryDate).IsRequired();
            entity.Property(e => e.IsRevoked).IsRequired();
            entity.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Token);
        });
    }
}

public class RefreshToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
}
```

**Migration**:
- Run `Add-Migration AddRefreshTokens` and `Update-Database` to create the `RefreshTokens` table in Firebird:
  ```sql
  CREATE TABLE RefreshTokens (
      Id VARCHAR(36) NOT NULL,
      UserId VARCHAR(36) NOT NULL,
      Token VARCHAR(512) NOT NULL,
      ExpiryDate TIMESTAMP NOT NULL,
      IsRevoked BOOLEAN NOT NULL DEFAULT FALSE,
      PRIMARY KEY (Id),
      FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
  );

  CREATE INDEX IDX_REFRESHTOKENS_USERID ON RefreshTokens (UserId);
  CREATE INDEX IDX_REFRESHTOKENS_TOKEN ON RefreshTokens (Token);
  ```

#### Step 2: Update AuthenticationRepository for Refresh Token Revocation
Add a method to `AuthenticationRepository` to revoke refresh tokens using EF Core instead of Dapper, since you're using EF Core for Identity.

In `ESLAdmin.Infrastructure.Repositories/AuthenticationRepository.cs`:

```csharp
using ErrorOr;
using ESLAdmin.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Repositories;

public partial class AuthenticationRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMessageLogger _messageLogger;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthenticationRepository(
        ApplicationDbContext dbContext,
        IMessageLogger messageLogger,
        UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _messageLogger = messageLogger;
        _userManager = userManager;
    }

    public async Task<ErrorOr<Success>> RevokeRefreshTokensAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _messageLogger.LogError("RevokeRefreshTokensAsync: User '{UserId}' not found.", userId);
                return Errors.IdentityErrors.UserNotFound(userId);
            }

            var tokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            if (!tokens.Any())
            {
                _messageLogger.LogInformation("No active refresh tokens found for user '{UserId}'.", userId);
                return Result.Success;
            }

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            _messageLogger.LogInformation("Revoked {TokenCount} refresh tokens for user '{UserId}'.", tokens.Count, userId);
            return Result.Success;
        }
        catch (DbUpdateException ex)
        {
            _messageLogger.LogError(ex, "Database error while revoking refresh tokens for user '{UserId}'.", userId);
            return Errors.IdentityErrors.TokenRevocationFailed(userId, ex.Message);
        }
        catch (OperationCanceledException ex)
        {
            _messageLogger.LogWarning(ex, "Operation canceled while revoking refresh tokens for user '{UserId}'.", userId);
            return Errors.IdentityErrors.OperationCanceled();
        }
        catch (Exception ex)
        {
            _messageLogger.LogError(ex, "Unexpected error while revoking refresh tokens for user '{UserId}'.", userId);
            return Errors.IdentityErrors.TokenRevocationFailed(userId, ex.Message);
        }
    }
}
```

- Uses EF Core to update `IsRevoked` for all active refresh tokens for the user.
- Validates user existence with `UserManager`.
- Handles `DbUpdateException` for database errors and `OperationCanceledException` for cancellations.
- Returns `ErrorOr<Success>` for consistency with your `ErrorOr` pattern.

#### Step 3: Update Error Definitions
Add error definitions to `ESLAdmin.Infrastructure.Errors/IdentityErrors.cs`:

```csharp
namespace ESLAdmin.Infrastructure.Errors;

public static class IdentityErrors
{
    public static Error UserNotFound(string userId) =>
        Error.NotFound("Identity.UserNotFound", $"User '{userId}' not found.");

    public static Error TokenRevocationFailed(string userId, string description) =>
        Error.Failure("Identity.TokenRevocationFailed", $"Failed to revoke tokens for user '{userId}': {description}");

    public static Error OperationCanceled() =>
        Error.Failure("Identity.OperationCanceled", "Operation was canceled.");
}
```

#### Step 4: Create LogoutCommand and Response
Define a command for logout. Since logout typically requires no input beyond the JWT token (which contains the user ID), the command can be empty.

In `ESLAdmin.Features.Endpoints.Auth/LogoutCommand.cs`:

```csharp
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Auth;

public class LogoutCommand : ICommand<Results<Ok, ProblemDetails>>
{
    // No parameters needed; user ID from JWT claims
}
```

#### Step 5: Create LogoutEndpoint
Create a FastEndpoints endpoint to handle the logout request, extracting the user ID from the JWT claims.

In `ESLAdmin.Features.Endpoints.Auth/LogoutEndpoint.cs`:

```csharp
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Auth;

public class LogoutEndpoint : Endpoint<LogoutCommand, Results<Ok, ProblemDetails>>
{
    private readonly IRepositoryManager _repositoryManager;

    public LogoutEndpoint(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public override void Configure()
    {
        Post("/api/auth/logout");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme); // Require JWT auth
        Description(b => b
            .Produces(StatusCodes.Status200OK)
            .ProducesProblemDetails(StatusCodes.Status401Unauthorized)
            .ProducesProblemDetails(StatusCodes.Status500InternalServerError));
    }

    public override async Task<Results<Ok, ProblemDetails>> ExecuteAsync(LogoutCommand req, CancellationToken ct)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return TypedResults.Problem(
                detail: "Invalid user ID in token",
                title: "Authentication Error",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return await new LogoutCommandInternal
        {
            UserId = userId
        }.ExecuteAsync(ct);
    }
}

internal class LogoutCommandInternal : ICommand<Results<Ok, ProblemDetails>>
{
    public string UserId { get; set; } = string.Empty;
}
```

- Requires JWT authentication via `AuthSchemes`.
- Extracts `userId` from the `sub` claim (standard for JWT).
- Delegates to an internal command for handler processing.

#### Step 6: Create LogoutCommandHandler
Create a handler to process the logout command, calling `RevokeRefreshTokensAsync`.

In `ESLAdmin.Features.Endpoints.Auth/LogoutCommandHandler.cs`:

```csharp
using ESLAdmin.Infrastructure.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Auth;

public class LogoutCommandHandler : ICommandHandler<
    LogoutCommandInternal,
    Results<Ok, ProblemDetails>>
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IRepositoryManager repositoryManager,
        ILogger<LogoutCommandHandler> logger)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
    }

    public async Task<Results<Ok, ProblemDetails>> ExecuteAsync(
        LogoutCommandInternal command,
        CancellationToken ct)
    {
        var result = await _repositoryManager.AuthenticationRepository.RevokeRefreshTokensAsync(command.UserId, ct);
        if (result.IsError)
        {
            _logger.LogWarning("Logout failed for user '{UserId}': {Error}", command.UserId, result.FirstError.Description);
            return TypedResults.Problem(
                detail: result.FirstError.Description,
                title: "Logout Failed",
                statusCode: result.FirstError.Type == ErrorType.NotFound
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation("Successful logout for user '{UserId}'", command.UserId);
        return TypedResults.Ok();
    }
}
```

- Calls `RevokeRefreshTokensAsync` to revoke tokens.
- Maps errors to `ProblemDetails` responses.
- Logs success and failure using `ILogger`.

#### Step 7: Update Refresh Token Validation
Ensure your refresh token endpoint checks for revoked tokens in EF Core.

In `AuthenticationRepository` (update `ValidateRefreshTokenAsync`):

```csharp
public async Task<ErrorOr<RefreshToken>> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
{
    try
    {
        var refreshToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

        if (refreshToken == null)
        {
            _messageLogger.LogError("ValidateRefreshTokenAsync: Refresh token not found.");
            return Errors.IdentityErrors.InvalidRefreshToken("Refresh token not found.");
        }

        if (refreshToken.IsRevoked || refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            _messageLogger.LogError("ValidateRefreshTokenAsync: Refresh token is revoked or expired for user '{UserId}'.", refreshToken.UserId);
            return Errors.IdentityErrors.InvalidRefreshToken("Refresh token is revoked or expired.");
        }

        var user = await _userManager.FindByIdAsync(refreshToken.UserId);
        if (user == null)
        {
            _messageLogger.LogError("ValidateRefreshTokenAsync: User '{UserId}' not found for refresh token.", refreshToken.UserId);
            return Errors.IdentityErrors.UserNotFound(refreshToken.UserId);
        }

        return new RefreshToken
        {
            UserId = refreshToken.UserId,
            Token = refreshToken.Token,
            ExpiryDate = refreshToken.ExpiryDate
        };
    }
    catch (OperationCanceledException ex)
    {
        _messageLogger.LogWarning(ex, "Operation canceled while validating refresh token.");
        return Errors.IdentityErrors.OperationCanceled();
    }
    catch (Exception ex)
    {
        _messageLogger.LogError(ex, "Unexpected error while validating refresh token.");
        return Errors.IdentityErrors.InvalidRefreshToken($"Unexpected error: {ex.Message}");
    }
}
```

Add to `IdentityErrors`:

```csharp
public static Error InvalidRefreshToken(string description) =>
    Error.Failure("Identity.InvalidRefreshToken", description);
```

#### Step 8: Update Refresh Token Generation
When generating a new refresh token during login or refresh, store it in the `RefreshTokens` table using EF Core.

In `AuthenticationRepository`:

```csharp
public async Task<ErrorOr<string>> GenerateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default)
{
    try
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _messageLogger.LogError("GenerateRefreshTokenAsync: User '{UserId}' not found.", userId);
            return Errors.IdentityErrors.UserNotFound(userId);
        }

        var token = Guid.NewGuid().ToString(); // Or use a secure random string
        var expiryDate = DateTime.UtcNow.AddDays(30);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiryDate = expiryDate,
            IsRevoked = false
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _messageLogger.LogInformation("Generated refresh token for user '{UserId}'.", userId);
        return token;
    }
    catch (DbUpdateException ex)
    {
        _messageLogger.LogError(ex, "Database error while generating refresh token for user '{UserId}'.", userId);
        return Errors.IdentityErrors.TokenGenerationFailed(userId, ex.Message);
    }
    catch (OperationCanceledException ex)
    {
        _messageLogger.LogWarning(ex, "Operation canceled while generating refresh token for user '{UserId}'.", userId);
        return Errors.IdentityErrors.OperationCanceled();
    }
    catch (Exception ex)
    {
        _messageLogger.LogError(ex, "Unexpected error while generating refresh token for user '{UserId}'.", userId);
        return Errors.IdentityErrors.TokenGenerationFailed(userId, ex.Message);
    }
}
```

Add to `IdentityErrors`:

```csharp
public static Error TokenGenerationFailed(string userId, string description) =>
    Error.Failure("Identity.TokenGenerationFailed", $"Failed to generate token for user '{userId}': {description}");
```

#### Step 9: Register Dependencies
Ensure all dependencies are registered in `Program.cs`:

```csharp
using ESLAdmin.Infrastructure.Data;
using ESLAdmin.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseFirebird(builder.Configuration.GetConnectionString("ESLAdminConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<AuthenticationRepository>();
builder.Services.AddSingleton<IMessageLogger, YourMessageLogger>();
```

#### Step 10: Test the Logout Function
1. **Login**: Use your login endpoint to get a JWT access token and refresh token.
2. **Logout**:
   - Send a POST request to `/api/auth/logout` with the JWT token in the `Authorization` header (`Bearer <token>`).
   - Example:
     ```bash
     curl -X POST http://localhost:5000/api/auth/logout \
     -H "Authorization: Bearer <your_jwt_token>"
     ```
3. **Verify**:
   - Check the `RefreshTokens` table; all tokens for the user should have `IsRevoked = TRUE`.
   - Attempt to refresh the token; it should fail with `InvalidRefreshToken`.
4. **Check Logs**:
   - Ensure `IMessageLogger` and `ILogger` logs show successful revocation or errors.

#### Additional Recommendations
1. **Access Token Blacklist**:
   - For immediate logout, implement a JWT blacklist (e.g., in Redis or a `TokenBlacklist` table). Add a middleware to check blacklisted tokens:
     ```csharp
     public class JwtBlacklistMiddleware
     {
         private readonly RequestDelegate _next;
         private readonly ApplicationDbContext _dbContext;

         public JwtBlacklistMiddleware(RequestDelegate next, ApplicationDbContext dbContext)
         {
             _next = next;
             _dbContext = dbContext;
         }

         public async Task InvokeAsync(HttpContext context)
         {
             if (context.User.Identity.IsAuthenticated)
             {
                 var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                 var isBlacklisted = await _dbContext.TokenBlacklist.AnyAsync(t => t.Token == token);
                 if (isBlacklisted)
                 {
                     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                     await context.Response.WriteAsync("Token is blacklisted.");
                     return;
                 }
             }
             await _next(context);
         }
     }
     ```
   - Add to `Program.cs`:
     ```csharp
     app.UseMiddleware<JwtBlacklistMiddleware>();
     ```

2. **Client-Side Handling**:
   - Instruct the client (e.g., web app) to clear JWT and refresh tokens from storage (e.g., `localStorage`) after a successful logout response.

3. **Security**:
   - Use HTTPS for the logout endpoint.
   - Consider single-use refresh tokens (replace the old token on each refresh).
   - Set a short JWT expiry (e.g., 15 minutes) to limit exposure after logout.

4. **Testing**:
   - Write unit tests for `RevokeRefreshTokensAsync`:
     ```csharp
     [Fact]
     public async Task RevokeRefreshTokensAsync_RevokesTokens_ReturnsSuccess()
     {
         var dbContext = new ApplicationDbContext(/* mock options */);
         var userManager = Substitute.For<UserManager<IdentityUser>>();
         var logger = Substitute.For<IMessageLogger>();
         userManager.FindByIdAsync("user1").Returns(new IdentityUser { Id = "user1" });
         dbContext.RefreshTokens.Add(new RefreshToken { Id = Guid.NewGuid().ToString(), UserId = "user1", Token = "token1", ExpiryDate = DateTime.UtcNow.AddDays(1), IsRevoked = false });
         await dbContext.SaveChangesAsync();

         var repo = new AuthenticationRepository(dbContext, logger, userManager);
         var result = await repo.RevokeRefreshTokensAsync("user1");

         Assert.False(result.IsError);
         Assert.True(dbContext.RefreshTokens.First().IsRevoked);
     }
     ```

5. **Firebird Optimization**:
   - Ensure indexes on `RefreshTokens` for performance:
     ```sql
     CREATE INDEX IDX_REFRESHTOKENS_USERID ON RefreshTokens (UserId);
     CREATE INDEX IDX_REFRESHTOKENS_TOKEN ON RefreshTokens (Token);
     ```

### Conclusion
This logout function uses EF Core Identity to revoke refresh tokens stored in a `RefreshTokens` table, integrating with FastEndpoints, Firebird, and your `ErrorOr` pattern. The endpoint (`/api/auth/logout`) extracts the user ID from the JWT, revokes tokens via `AuthenticationRepository`, and returns appropriate responses. It aligns with your existing `DbContextDapper`, `IConfigurationParams`, and `IMessageLogger` usage, ensuring consistency with login and refresh token flows.

If you need help with specific configurations (e.g., JWT claim names, `IMessageLogger` implementation, or access token blacklisting), or want to share parts of your login/refresh code for integration, let me know!