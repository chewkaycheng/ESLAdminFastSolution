In Firebird, the error about the token field being "too big to create index on" typically occurs when trying to create an index on a column (like the `Token` field in the `BlacklistedTokens` or `RefreshTokens` table) that exceeds Firebird's index key size limit. Firebird has a maximum index key size of **approximately 252 bytes** for a single-column index (or one-quarter of the page size, which is typically 4096 bytes by default). If your `Token` field is defined as `VARCHAR(512)` or `VARCHAR(2048)` (as in the JWT blacklist implementation), a JWT or refresh token (which can be hundreds of characters long) may exceed this limit, causing the index creation to fail.

To address this, we’ll modify the JWT blacklist implementation from the previous response to avoid indexing the full `Token` field directly. Instead, we’ll store a **hash** (e.g., SHA-256) of the token in a separate column and index that, as hashes are fixed-length (32 bytes for SHA-256, well within Firebird’s limit). This approach maintains performance for lookups while keeping the token data intact for auditing or debugging. The solution will integrate with your `ESLAdmin` project, using EF Core, FastEndpoints, ASP.NET Core Identity, Firebird, and the `ErrorOr` pattern.

### Why the Issue Occurs
- **Firebird Index Limit**: Firebird’s maximum index key size is approximately 252 bytes (for a 4KB page size). A `VARCHAR(512)` or `VARCHAR(2048)` column storing a JWT or refresh token often exceeds this, especially with UTF-8 encoding (where each character can use 1–4 bytes).
- **JWT/Refresh Token Size**: JWTs and refresh tokens can be long (e.g., 200–1000+ characters), especially with large payloads or signatures, making direct indexing impractical.
- **Solution**: Store a SHA-256 hash of the token (32 bytes) in a `TokenHash` column, index it, and use it for lookups. Store the full token for reference but don’t index it.

### Step-by-Step Solution

#### Step 1: Update the BlacklistedToken Entity
Modify the `BlacklistedToken` entity to include a `TokenHash` column for the SHA-256 hash of the token, which will be indexed instead of the `Token` column.

In `ESLAdmin.Models/BlacklistedToken.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Models;

public class BlacklistedToken
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Unique ID as primary key
    public string Token { get; set; } = string.Empty; // Full JWT (not indexed)
    [Required, MaxLength(64)] // SHA-256 hash in hex (64 chars)
    public string TokenHash { get; set; } = string.Empty; // Indexed hash
    public DateTime ExpiryDate { get; set; } // Token's original expiration
    public string UserId { get; set; } = string.Empty; // Link to user
    public DateTime BlacklistedOn { get; set; } = DateTime.UtcNow; // Timestamp
}
```

- **Changes**:
  - Replaced `Token` as the primary key with `Id` (GUID) to avoid indexing the token directly.
  - Added `TokenHash` (64 characters for SHA-256 hex) for indexing.
  - Kept `Token` for storing the full JWT (not indexed).

#### Step 2: Update ApplicationDbContext
Update the `ApplicationDbContext` to configure the `BlacklistedTokens` table with an index on `TokenHash` instead of `Token`.

In `ESLAdmin.Infrastructure.Data/ApplicationDbContext.cs`:

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ESLAdmin.Models;

namespace ESLAdmin.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BlacklistedToken>(entity =>
        {
            entity.HasKey(e => e.Id); // Primary key on Id
            entity.Property(e => e.Token).IsRequired().HasMaxLength(2048); // Store full token
            entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(64); // SHA-256 hash
            entity.Property(e => e.UserId).HasMaxLength(450); // Match IdentityUser Id
            entity.HasIndex(e => e.TokenHash).IsUnique(); // Index on TokenHash
            entity.HasIndex(e => e.UserId); // Index for user queries
            entity.HasIndex(e => e.ExpiryDate); // Index for cleanup
        });
    }
}
```

- **Changes**:
  - Set `Id` as the primary key.
  - Added `TokenHash` with a unique index (64 bytes fits within Firebird’s limit).
  - Removed index on `Token` to avoid the size error.
  - Kept indexes on `UserId` and `ExpiryDate` for query performance.

**Run Migrations**:
- Generate and apply migrations to update the database schema:
  ```bash
  dotnet ef migrations add UpdateBlacklistedTokens
  dotnet ef database update
  ```
- **Generated Table** (approximate Firebird SQL):
  ```sql
  CREATE TABLE BlacklistedTokens (
      Id VARCHAR(36) NOT NULL,
      Token VARCHAR(2048) NOT NULL,
      TokenHash VARCHAR(64) NOT NULL,
      ExpiryDate TIMESTAMP NOT NULL,
      UserId VARCHAR(450) NOT NULL,
      BlacklistedOn TIMESTAMP NOT NULL,
      PRIMARY KEY (Id)
  );

  CREATE UNIQUE INDEX IDX_BLACKLISTEDTOKENS_TOKENHASH ON BlacklistedTokens (TokenHash);
  CREATE INDEX IDX_BLACKLISTEDTOKENS_USERID ON BlacklistedTokens (UserId);
  CREATE INDEX IDX_BLACKLISTEDTOKENS_EXPIRYDATE ON BlacklistedTokens (ExpiryDate);
  ```

#### Step 3: Update TokenBlacklistService
Modify the `TokenBlacklistService` to compute and store the SHA-256 hash of the token and use it for lookups.

In `ESLAdmin.Infrastructure.Services/TokenBlacklistService.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ESLAdmin.Infrastructure.Data;
using ESLAdmin.Models;

namespace ESLAdmin.Infrastructure.Services;

public interface ITokenBlacklistService
{
    Task AddToBlacklistAsync(string token, string userId, DateTime expiryDate, CancellationToken cancellationToken = default);
    Task<bool> IsBlacklistedAsync(string token, CancellationToken cancellationToken = default);
    Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
}

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly ApplicationDbContext _context;

    public TokenBlacklistService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddToBlacklistAsync(string token, string userId, DateTime expiryDate, CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeSha256Hash(token);
        var blacklistedToken = new BlacklistedToken
        {
            Token = token,
            TokenHash = tokenHash,
            UserId = userId,
            ExpiryDate = expiryDate,
            BlacklistedOn = DateTime.UtcNow
        };

        _context.BlacklistedTokens.Add(blacklistedToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsBlacklistedAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenHash = ComputeSha256Hash(token);
        return await _context.BlacklistedTokens
            .AnyAsync(bt => bt.TokenHash == tokenHash, cancellationToken);
    }

    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _context.BlacklistedTokens
            .Where(bt => bt.ExpiryDate < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        _context.BlacklistedTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static string ComputeSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant(); // 64 chars
    }
}
```

- **Changes**:
  - Added `ComputeSha256Hash` to generate a SHA-256 hash (64 characters in hex).
  - Store `TokenHash` alongside `Token` in `AddToBlacklistAsync`.
  - Check `TokenHash` in `IsBlacklistedAsync` for efficient lookups.
  - Added `CancellationToken` support for consistency with your async patterns.

**Register in Program.cs**:
```csharp
builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
```

#### Step 4: Update LogoutCommandHandler to Blacklist JWT
Modify the `LogoutCommandHandler` to blacklist the JWT token by calling `AddToBlacklistAsync`. This assumes you’re using the logout endpoint and handler from the previous response, integrated with your refresh token revocation.

In `ESLAdmin.Features.Endpoints.Auth/LogoutCommandHandler.cs`:

```csharp
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Infrastructure.Services;

namespace ESLAdmin.Features.Endpoints.Auth;

public class LogoutCommandHandler : ICommandHandler<
    LogoutCommandInternal,
    Results<Ok, ProblemDetails>>
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IRepositoryManager repositoryManager,
        ITokenBlacklistService tokenBlacklistService,
        ILogger<LogoutCommandHandler> logger)
    {
        _repositoryManager = repositoryManager;
        _tokenBlacklistService = tokenBlacklistService;
        _logger = logger;
    }

    public async Task<Results<Ok, ProblemDetails>> ExecuteAsync(
        LogoutCommandInternal command,
        CancellationToken ct)
    {
        // Blacklist JWT
        var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogWarning("Logout failed: No JWT provided for user '{UserId}'.", command.UserId);
            return TypedResults.Problem(
                detail: "No JWT provided.",
                title: "Authentication Error",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(accessToken))
        {
            _logger.LogWarning("Logout failed: Invalid JWT format for user '{UserId}'.", command.UserId);
            return TypedResults.Problem(
                detail: "Invalid JWT format.",
                title: "Authentication Error",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var jwtToken = handler.ReadJwtToken(accessToken);
        var expiryDate = jwtToken.ValidTo;

        try
        {
            // Revoke refresh tokens
            var revokeResult = await _repositoryManager.AuthenticationRepository.RevokeRefreshTokensAsync(command.UserId, ct);
            if (revokeResult.IsError)
            {
                _logger.LogWarning("Logout failed for user '{UserId}': {Error}", command.UserId, revokeResult.FirstError.Description);
                return TypedResults.Problem(
                    detail: revokeResult.FirstError.Description,
                    title: "Logout Failed",
                    statusCode: revokeResult.FirstError.Type == ErrorType.NotFound
                        ? StatusCodes.Status404NotFound
                        : StatusCodes.Status500InternalServerError);
            }

            // Blacklist JWT
            await _tokenBlacklistService.AddToBlacklistAsync(accessToken, command.UserId, expiryDate, ct);

            _logger.LogInformation("Successful logout for user '{UserId}'. JWT blacklisted.", command.UserId);
            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during logout for user '{UserId}'.", command.UserId);
            return TypedResults.Problem(
                detail: ex.Message,
                title: "Logout Failed",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
```

- **Changes**:
  - Validates the JWT token format.
  - Adds the token to the blacklist using `AddToBlacklistAsync`.
  - Combines refresh token revocation (from previous implementation) with JWT blacklisting.
  - Handles errors with `ProblemDetails` and `ErrorOr`.

#### Step 5: Update JwtBlacklistMiddleware
Update the middleware to check the `TokenHash` column for blacklisted tokens.

In `ESLAdmin.Infrastructure.Middleware/JwtBlacklistMiddleware.cs`:

```csharp
using Microsoft.AspNetCore.Http;
using ESLAdmin.Infrastructure.Services;

namespace ESLAdmin.Infrastructure.Middleware;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public JwtBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlacklistService tokenBlacklistService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token) && await tokenBlacklistService.IsBlacklistedAsync(token, context.RequestAborted))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token has been revoked.");
                return;
            }
        }

        await _next(context);
    }
}
```

- **Changes**:
  - Uses `IsBlacklistedAsync`, which checks `TokenHash`.
  - Added `context.RequestAborted` as the `CancellationToken` for async cancellation support.
  - Registered after `UseAuthentication` and before `UseAuthorization` in `Program.cs`:
    ```csharp
    app.UseAuthentication();
    app.UseMiddleware<JwtBlacklistMiddleware>();
    app.UseAuthorization();
    ```

#### Step 6: Update RefreshTokens for Consistency
For consistency, update the `RefreshTokens` table and related logic to use a `TokenHash` column, as refresh tokens may also exceed Firebird’s index size limit.

In `ESLAdmin.Models/RefreshToken.cs`:

```csharp
namespace ESLAdmin.Models;

public class RefreshToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty; // SHA-256 hash
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
}
```

Update `ApplicationDbContext` for `RefreshTokens`:

```csharp
modelBuilder.Entity<RefreshToken>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.UserId).IsRequired();
    entity.Property(e => e.Token).IsRequired().HasMaxLength(2048);
    entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(64);
    entity.Property(e => e.ExpiryDate).IsRequired();
    entity.Property(e => e.IsRevoked).IsRequired();
    entity.HasOne<IdentityUser>()
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    entity.HasIndex(e => e.TokenHash).IsUnique();
    entity.HasIndex(e => e.UserId);
});
```

Update `AuthenticationRepository` methods (`GenerateRefreshTokenAsync` and `ValidateRefreshTokenAsync`):

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
        var tokenHash = ComputeSha256Hash(token);
        var expiryDate = DateTime.UtcNow.AddDays(30);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            TokenHash = tokenHash,
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

public async Task<ErrorOr<RefreshToken>> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
{
    try
    {
        var tokenHash = ComputeSha256Hash(token);
        var refreshToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

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
            TokenHash = refreshToken.TokenHash,
            ExpiryDate = refreshToken.ExpiryDate,
            IsRevoked = refreshToken.IsRevoked
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

private static string ComputeSha256Hash(string input)
{
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(input);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToHexString(hash).ToLowerInvariant();
}
```

**Run Migrations**:
```bash
dotnet ef migrations add UpdateRefreshTokensWithTokenHash
dotnet ef database update
```

#### Step 7: Update RevokeRefreshTokensAsync
Update the `RevokeRefreshTokensAsync` method to use `TokenHash` for consistency.

In `ESLAdmin.Infrastructure.Repositories/AuthenticationRepository.cs`:

```csharp
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
```

- No changes needed here, as it uses `UserId` for revocation, but included for completeness.

#### Step 8: Periodic Cleanup of Expired Tokens
Schedule a background job to clean up expired tokens from `BlacklistedTokens` and `RefreshTokens` to manage database size.

In `ESLAdmin.Infrastructure.Services/BackgroundTokenCleanupService.cs`:

```csharp
using Microsoft.Extensions.Hosting;
using ESLAdmin.Infrastructure.Data;
using ESLAdmin.Infrastructure.Services;

namespace ESLAdmin.Infrastructure.Services;

public class BackgroundTokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundTokenCleanupService> _logger;

    public BackgroundTokenCleanupService(
        IServiceProvider serviceProvider,
        ILogger<BackgroundTokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var tokenBlacklistService = scope.ServiceProvider.GetRequiredService<ITokenBlacklistService>();
                await tokenBlacklistService.CleanupExpiredTokensAsync(stoppingToken);
                _logger.LogInformation("Cleaned up expired blacklisted tokens.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token cleanup.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Run daily
        }
    }
}
```

**Register in Program.cs**:
```csharp
builder.Services.AddHostedService<BackgroundTokenCleanupService>();
```

#### Step 9: Test the Implementation
1. **Login**: Obtain a JWT and refresh token.
2. **Logout**:
   - Send POST to `/api/auth/logout` with `Authorization: Bearer <jwt>`.
   - Verify `BlacklistedTokens` has a new record with `TokenHash` and `Token`.
   - Verify `RefreshTokens` has `IsRevoked = TRUE` for the user’s tokens.
3. **Test Blacklist**:
   - Use the blacklisted JWT in a protected endpoint; expect a 401 response.
4. **Test Refresh Token**:
   - Attempt to refresh with a revoked token; expect an error.
5. **Check Cleanup**:
   - Manually set an old `ExpiryDate` in `BlacklistedTokens` and run `CleanupExpiredTokensAsync`; verify removal.

**Example Test**:
```csharp
[Fact]
public async Task Logout_BlacklistsJwt_ReturnsOk()
{
    var dbContext = new ApplicationDbContext(/* mock options */);
    var tokenBlacklistService = new TokenBlacklistService(dbContext);
    var logger = Substitute.For<ILogger<LogoutCommandHandler>>();
    var repoManager = Substitute.For<IRepositoryManager>();
    repoManager.AuthenticationRepository.RevokeRefreshTokensAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(Task.FromResult(ErrorOr<Success>.From(Result.Success)));

    var handler = new LogoutCommandHandler(repoManager, tokenBlacklistService, logger);
    var context = Substitute.For<HttpContext>();
    context.Request.Headers["Authorization"].Returns("Bearer eyJ...");

    var result = await handler.ExecuteAsync(new LogoutCommandInternal { UserId = "user1" }, default);

    Assert.IsType<Ok>(result);
    Assert.True(await dbContext.BlacklistedTokens.AnyAsync(bt => bt.UserId == "user1"));
}
```

#### Step 10: Firebird-Specific Considerations
- **Index Size**: The `TokenHash` column (64 characters, ~64 bytes in UTF-8) is well within Firebird’s 252-byte index limit, resolving the original issue.
- **Performance**: Indexes on `TokenHash`, `UserId`, and `ExpiryDate` ensure fast lookups and cleanup.
- **Firebird Encoding**: Use `UTF8` character set for `Token` and `TokenHash` to handle JWT characters correctly:
  ```sql
  ALTER TABLE BlacklistedTokens ALTER TokenHash TYPE VARCHAR(64) CHARACTER SET UTF8;
  ```

#### Additional Recommendations
1. **Security**:
   - Use HTTPS for all endpoints.
   - Consider single-use refresh tokens (replace on each refresh) to limit exposure.
2. **Cleanup Frequency**: Adjust the cleanup interval (`TimeSpan.FromHours(24)`) based on your token volume.
3. **Logging**: Enhance `IMessageLogger` to log token hashes (not full tokens) for security:
    ```csharp
    _messageLogger.LogInformation("Blacklisted token with hash '{TokenHash}' for user '{UserId}'.", tokenHash, userId);
    ```
4. **Alternative to Hashing**: If hashing adds complexity, consider truncating tokens to fit within 252 bytes, but this risks collisions. Hashing is safer.

### Conclusion
By storing a SHA-256 hash of the JWT in a `TokenHash` column and indexing it, you bypass Firebird’s index size limit while maintaining performance. The updated blacklist implementation integrates with your EF Core Identity setup, FastEndpoints, and refresh token revocation, ensuring secure logout. The middleware checks blacklisted tokens efficiently, and periodic cleanup prevents database bloat.

If you need help with migrations, specific JWT configurations, or integrating with other parts of your `ESLAdmin` project (e.g., `IMessageLogger`, login flow), share details, and I can refine further!