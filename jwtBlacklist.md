### Step-by-Step Guide to Implementing JWT Blacklist Using EF Core

Implementing a JWT blacklist allows you to invalidate JWT tokens immediately upon logout (or other events like token revocation), as JWTs are stateless and can't be expired server-side without a mechanism like blacklisting. We'll use EF Core to store blacklisted tokens in a database table. This guide assumes you have an ASP.NET Core project with EF Core set up for Identity (e.g., `ApplicationDbContext` inheriting from `IdentityDbContext`), JWT authentication configured, and a logout endpoint. We'll focus on the blacklist feature.

#### Step 1: Define the Blacklist Entity
Create an entity class to represent blacklisted tokens. This will map to a database table storing the token, expiration date (for cleanup), and optionally the user ID for reference.

In a new file, e.g., `Models/BlacklistedToken.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace YourProject.Models;

public class BlacklistedToken
{
    [Key]
    public string Token { get; set; } = string.Empty; // JWT token as primary key
    public DateTime ExpiryDate { get; set; } // Token's original expiration for cleanup
    public string UserId { get; set; } = string.Empty; // Optional: Link to user for auditing
    public DateTime BlacklistedOn { get; set; } = DateTime.UtcNow; // When it was blacklisted
}
```

- **Why these properties?**
  - `Token`: The JWT string to check against.
  - `ExpiryDate`: Store the token's `exp` claim to allow periodic cleanup of expired tokens.
  - `UserId`: For logging or querying blacklists per user.
  - `BlacklistedOn`: Timestamp for auditing.

#### Step 2: Update DbContext to Generate the Table
Add the `BlacklistedToken` entity to your EF Core `DbContext` (e.g., `ApplicationDbContext`). This will generate the table during migrations.

In `Data/ApplicationDbContext.cs` (assuming it inherits from `IdentityDbContext`):

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YourProject.Models;

namespace YourProject.Data;

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
            entity.HasKey(e => e.Token); // Primary key on Token
            entity.Property(e => e.Token).IsRequired().HasMaxLength(2048); // Adjust length for JWT size
            entity.Property(e => e.UserId).HasMaxLength(450); // Match IdentityUser Id length
            entity.HasIndex(e => e.UserId); // Index for querying by user
            entity.HasIndex(e => e.ExpiryDate); // Index for cleanup queries
        });
    }
}
```

- **Run Migrations**: In the Package Manager Console or terminal:
  ```
  dotnet ef migrations add AddBlacklistedTokens
  dotnet ef database update
  ```
- **Generated Table**: This creates a `BlacklistedTokens` table in your database (e.g., Firebird) with the defined columns and indexes.

#### Step 3: Create a Service to Manage Blacklist
Create a service to add tokens to the blacklist and check if a token is blacklisted. This can be a scoped service injected into your logout handler and middleware.

In `Services/TokenBlacklistService.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using YourProject.Data;
using YourProject.Models;

namespace YourProject.Services;

public interface ITokenBlacklistService
{
    Task AddToBlacklistAsync(string token, string userId, DateTime expiryDate);
    Task<bool> IsBlacklistedAsync(string token);
    Task CleanupExpiredTokensAsync(); // Optional: For periodic cleanup
}

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly ApplicationDbContext _context;

    public TokenBlacklistService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddToBlacklistAsync(string token, string userId, DateTime expiryDate)
    {
        var blacklistedToken = new BlacklistedToken
        {
            Token = token,
            UserId = userId,
            ExpiryDate = expiryDate,
            BlacklistedOn = DateTime.UtcNow
        };

        _context.BlacklistedTokens.Add(blacklistedToken);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsBlacklistedAsync(string token)
    {
        return await _context.BlacklistedTokens.AnyAsync(bt => bt.Token == token);
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context.BlacklistedTokens
            .Where(bt => bt.ExpiryDate < DateTime.UtcNow)
            .ToListAsync();

        _context.BlacklistedTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}
```

- **Register in Program.cs**:
  ```csharp
  builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
  ```

#### Step 4: Write JWT to Blacklist on Logout
In your logout command handler (e.g., `LogoutCommandHandler`), extract the JWT token from the request, parse its expiry, and add it to the blacklist.

In `Features/Endpoints/Auth/LogoutCommandHandler.cs`:

```csharp
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using YourProject.Services;

namespace YourProject.Features.Endpoints.Auth;

public class LogoutCommandHandler : ICommandHandler<
    LogoutCommandInternal,
    Results<Ok, ProblemDetails>>
{
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        ITokenBlacklistService tokenBlacklistService,
        ILogger<LogoutCommandHandler> logger)
    {
        _tokenBlacklistService = tokenBlacklistService;
        _logger = logger;
    }

    public async Task<Results<Ok, ProblemDetails>> ExecuteAsync(
        LogoutCommandInternal command,
        CancellationToken ct)
    {
        var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        var expiryDate = jwtToken.ValidTo;

        await _tokenBlacklistService.AddToBlacklistAsync(accessToken, command.UserId, expiryDate);

        _logger.LogInformation("JWT blacklisted for user '{UserId}' on logout.", command.UserId);
        return TypedResults.Ok();
    }
}
```

- Extracts the JWT from the `Authorization` header.
- Parses the expiry date from the token's `ValidTo` property.
- Adds the token to the blacklist using `ITokenBlacklistService`.

#### Step 5: Create Middleware to Check for Blacklisted Tokens
Add middleware to check if the incoming JWT is blacklisted during authentication. Place it after authentication middleware but before authorization.

In `Middleware/JwtBlacklistMiddleware.cs`:

```csharp
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using YourProject.Services;

namespace YourProject.Middleware;

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
            if (!string.IsNullOrEmpty(token) && await tokenBlacklistService.IsBlacklistedAsync(token))
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

- **Register in Program.cs** (after authentication middleware):
  ```csharp
  app.UseAuthentication();
  app.UseMiddleware<JwtBlacklistMiddleware>();
  app.UseAuthorization();
  ```

#### Step 6: Test the Implementation
1. **Login**: Obtain a JWT token.
2. **Logout**: Call the logout endpoint; verify the token is added to the `BlacklistedTokens` table.
3. **Access Protected Endpoint**: Use the blacklisted token; expect a 401 Unauthorized response.
4. **Cleanup**: Run `tokenBlacklistService.CleanupExpiredTokensAsync()` periodically (e.g., via a background service) to remove expired tokens.

### Conclusion
This guide implements a JWT blacklist with EF Core by creating a `BlacklistedToken` entity and table, adding tokens on logout, and checking them via middleware. It integrates with your FastEndpoints command bus and Identity setup, ensuring secure logout. If needed, add cleanup for expired tokens in a background job.

If you have specific details (e.g., JWT configuration or Firebird schema), I can refine this further!