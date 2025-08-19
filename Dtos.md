You're using **ASP.NET Web API 9.0** with **FastEndpoints**, **EF Core**, **Dapper**, **Firebird**, and **Vertical Slice Architecture**. You’ve provided the `GetUserRequest` and `GetUserResponse` DTOs for a `GetUser` endpoint, and a `GetUserCommand` for the corresponding Command Handler. The `GetUserCommand` implements FastEndpoints' `ICommand<Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>>`, indicating the Command Handler should return a result that can be one of:
- `Ok<GetUserResponse>`: Success with a `GetUserResponse` object.
- `ProblemDetails`: Client or validation error (e.g., user not found).
- `InternalServerError`: Unexpected server error.

Your question is: **What object/class should the Command Handler return?**

### Analysis
- **Context**:
  - The `GetUserRequest` contains an `Email` field, used as input to fetch a user.
  - The `GetUserResponse` contains user details (`Id`, `FirstName`, `LastName`, `UserName`, `Email`, `PhoneNumber`, `Roles`), returned to the client.
  - The `GetUserCommand` mirrors the `GetUserRequest` with an `Email` field, used to pass the business operation to the Command Handler.
  - Your `IdentityRepository` (from previous context, `artifact_id: be994219-57df-472d-806b-9189a37c1f02`) likely has a method to fetch a user by email, returning an `ErrorOr<User>` (e.g., `FindByEmailAsync` from `UserManager<User>` or a custom method).
  - The Command Handler must bridge the endpoint and repository, mapping the `User` entity to a `GetUserResponse` and handling errors appropriately.

- **FastEndpoints Command Handler**:
  - The `ICommand<Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>>` indicates the handler’s `ExecuteAsync` method returns a `Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>`.
  - This is a **FluentResults** type, allowing multiple result types:
    - `Ok<GetUserResponse>`: Wraps a `GetUserResponse` for successful cases.
    - `ProblemDetails`: Represents client errors (e.g., 400 Bad Request, 404 Not Found) with a structured error response.
    - `InternalServerError`: Represents server errors (e.g., 500 Internal Server Error) for unexpected failures.

- **Repository Interaction**:
  - The repository likely returns an `ErrorOr<User>` (based on your `IdentityRepository` methods like `LoginAsync`, `ValidateRefreshTokenAsync`).
  - The handler maps the `User` entity to a `GetUserResponse` for success or converts `ErrorOr` errors to `ProblemDetails` or `InternalServerError`.

- **What to Return**:
  - The Command Handler should return a `Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>` object, as specified by the `ICommand` interface.
  - For success: Return `Results.Ok(new GetUserResponse { ... })`, mapping the `User` entity to the response DTO.
  - For client errors (e.g., user not found): Return a `ProblemDetails` with appropriate status code and message (e.g., 404 for `UserEmailNotFound`).
  - For server errors (e.g., database failure): Return an `InternalServerError`.

### Recommendation
The Command Handler should:
1. Accept the `GetUserCommand` with the `Email`.
2. Call the repository to fetch the `User` by email (e.g., `_userManager.FindByEmailAsync` or a custom `GetUserByEmailAsync`).
3. Map the `User` to a `GetUserResponse` for success.
4. Convert repository errors (e.g., `ErrorOr.Error`) to `ProblemDetails` or `InternalServerError`.
5. Return `Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>`.

### Example Implementation
I’ll provide a `GetUserCommandHandler` that interacts with `IdentityRepository`, maps the `User` entity to `GetUserResponse`, and returns the required result type. I’ll also add a `GetUserByEmailAsync` method to `IdentityRepository` and create the endpoint. Existing artifacts (`IdentityRepository.cs`, `AppErrors.cs`) will be reused or updated.

#### AppErrors.cs
Ensure `AppErrors.cs` includes necessary errors (`artifact_id: c264d344-7805-40eb-897e-8726b6e187b6`).

<xaiArtifact artifact_id="c264d344-7805-40eb-897e-8726b6e187b6" artifact_version_id="dd2d45f2-2b5f-4710-a340-2fbed2344689" title="AppErrors.cs" contentType="text/plain">
namespace ESLAdmin.Common.CustomErrors;

public static partial class AppErrors
{
    public static class CommonErrors
    {
        public static ErrorOr.Error ConfigurationMissingKey(string key) =>
            ErrorOr.Error.Failure($"Configuration key '{key}' is missing or empty.");

        public static ErrorOr.Error Unauthorized() =>
            ErrorOr.Error.Unauthorized(code: "Auth.Unauthorized", description: "User is not authorized.");

        public static ErrorOr.Error Exception(string message) =>
            ErrorOr.Error.Failure($"An unexpected error occurred: {message}");
    }

    public static class IdentityErrors
    {
        public static ErrorOr.Error UserEmailCannotBeEmpty() =>
            ErrorOr.Error.Validation("User email cannot be empty.");

        public static ErrorOr.Error InvalidRoles(ICollection<string> invalidRoles) =>
            ErrorOr.Error.Validation($"Invalid roles: {string.Join(", ", invalidRoles)}");

        public static ErrorOr.Error DuplicateUserName(string userName) =>
            ErrorOr.Error.Conflict($"Username '{userName}' is already taken.");

        public static ErrorOr.Error DuplicateEmail(string email) =>
            ErrorOr.Error.Conflict($"Email '{email}' is already registered.");

        public static ErrorOr.Error InvalidUserName(string userName) =>
            ErrorOr.Error.Validation($"Invalid username: '{userName}'.");

        public static ErrorOr.Error InvalidEmail(string email) =>
            ErrorOr.Error.Validation($"Invalid email: '{email}'.");

        public static ErrorOr.Error UserEmailNotFound(string email) =>
            ErrorOr.Error.NotFound($"User with email '{email}' not found.");

        public static ErrorOr.Error UserNotFound(string userId) =>
            ErrorOr.Error.NotFound($"User with ID '{userId}' not found.");

        public static ErrorOr.Error RoleNotFound(string role) =>
            ErrorOr.Error.NotFound($"Role '{role}' not found.");

        public static ErrorOr.Error UserAlreadyInRole(string userId, string roles) =>
            ErrorOr.Error.Conflict($"User '{userId}' is already in role(s): {roles}.");

        public static ErrorOr.Error ConcurrencyFailure(string id) =>
            ErrorOr.Error.Conflict($"Concurrency failure for user or role with ID '{id}'.");

        public static ErrorOr.Error CreateUserFailed(string userName, string email, IEnumerable<IdentityError> errors) =>
            ErrorOr.Error.Failure($"Failed to create user '{userName}' with email '{email}': {errors.ToFormattedString()}");

        public static ErrorOr.Error AddToRolesFailed(string userId, ICollection<string> roles, IEnumerable<IdentityError> errors) =>
            ErrorOr.Error.Failure($"Failed to add user '{userId}' to roles '{string.Join(", ", roles)}': {errors.ToFormattedString()}");

        public static ErrorOr.Error DeleteUserFailed(string userId, IEnumerable<IdentityError> errors) =>
            ErrorOr.Error.Failure($"Failed to delete user with ID '{userId}': {errors.ToFormattedString()}");

        public static ErrorOr.Error InvalidArgument(string message) =>
            ErrorOr.Error.Validation($"Invalid argument: {message}");

        public static ErrorOr.Error UserLockedOut(string email) =>
            ErrorOr.Error.Failure($"User with email '{email}' is locked out.");

        public static ErrorOr.Error UserNotAllowed(string email) =>
            ErrorOr.Error.Failure($"User with email '{email}' is not allowed to sign in.");

        public static ErrorOr.Error RequiresTwoFactor(string email) =>
            ErrorOr.Error.Failure($"User with email '{email}' requires two-factor authentication.");

        public static ErrorOr.Error InvalidCredentials(string email) =>
            ErrorOr.Error.Unauthorized($"Invalid credentials for user with email '{email}'.");

        public static ErrorOr.Error RefreshTokenNotFound(string token) =>
            ErrorOr.Error.NotFound($"Refresh token not found.");

        public static ErrorOr.Error InvalidRefreshToken() =>
            ErrorOr.Error.Unauthorized($"Invalid or expired refresh token.");
    }

    public static class DatabaseErrors
    {
        public static ErrorOr.Error DatabaseError(string message) =>
            ErrorOr.Error.Failure($"Database error: {message}");

        public static ErrorOr.Error OperationCanceled() =>
            ErrorOr.Error.Failure("Operation was canceled.");

        public static ErrorOr.Error NoChangesSaved() =>
            ErrorOr.Error.Failure("No changes were saved to the database.");
    }
}
</xaiArtifact>

**Changes**: No changes needed, as errors like `UserEmailNotFound` and `DatabaseError` are already defined.

#### IdentityRepository.cs
Add `GetUserByEmailAsync` to fetch a user and their roles, returning `ErrorOr<User>`.

<xaiArtifact artifact_id="be994219-57df-472d-806b-9189a37c1f02" artifact_version_id="e5372a51-5b2d-40f1-a503-0cfeb1d0c16b" title="IdentityRepository.cs" contentType="text/plain">
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Persistence.Repositories;

public interface IIdentityRepository
{
    Task<ErrorOr<User>> RegisterUserAsync(User user, string password, ICollection<string>? roles);
    Task<ErrorOr<string>> DeleteUserByEmailAsync(string email);
    Task<ErrorOr<User>> LoginAsync(string email, string password, bool lockoutOnFailure);
    Task<ErrorOr<User>> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task<ErrorOr<RefreshToken>> IssueRefreshTokenAsync(string userId, CancellationToken cancellationToken);
    Task<ErrorOr<bool>> RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task<ErrorOr<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
}

public static class DatabaseExceptionHandler
{
    public static ErrorOr.Error HandleException(Exception ex, ILogger logger)
    {
        return ex switch
        {
            DbUpdateConcurrencyException => AppErrors.DatabaseErrors.NoChangesSaved(),
            DbUpdateException dbEx => AppErrors.DatabaseErrors.DatabaseError(dbEx.InnerException?.Message ?? dbEx.Message),
            OperationCanceledException => AppErrors.DatabaseErrors.OperationCanceled(),
            _ => AppErrors.CommonErrors.Exception(ex.Message)
        };
    }
}

public class IdentityRepository : IIdentityRepository
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly UserDbContext _dbContext;
    private readonly ILogger<IdentityRepository> _logger;

    public IdentityRepository(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        SignInManager<User> signInManager,
        UserDbContext dbContext,
        ILogger<IdentityRepository> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ErrorOr<User>> RegisterUserAsync(User user, string password, ICollection<string>? roles)
    {
        _logger.LogInformation("Registering user with email {Email}", user.Email);

        if (string.IsNullOrEmpty(user.Email))
        {
            _logger.LogWarning("User email cannot be empty");
            return AppErrors.IdentityErrors.UserEmailCannotBeEmpty();
        }

        // Check for invalid roles
        if (roles != null && roles.Any())
        {
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            var invalidRoles = roles.Except(allRoles).ToList();
            if (invalidRoles.Any())
            {
                _logger.LogWarning("Invalid roles provided: {InvalidRoles}", string.Join(", ", invalidRoles));
                return AppErrors.IdentityErrors.InvalidRoles(invalidRoles);
            }
        }

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Failed to create user with email {Email}: {Errors}", user.Email, result.Errors.ToFormattedString());
                var firstError = result.Errors.FirstOrDefault();
                return firstError?.Code switch
                {
                    "DuplicateUserName" => AppErrors.IdentityErrors.DuplicateUserName(user.UserName),
                    "DuplicateEmail" => AppErrors.IdentityErrors.DuplicateEmail(user.Email),
                    "InvalidUserName" => AppErrors.IdentityErrors.InvalidUserName(user.UserName),
                    "InvalidEmail" => AppErrors.IdentityErrors.InvalidEmail(user.Email ?? "null"),
                    "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(user.UserName),
                    _ => AppErrors.IdentityErrors.CreateUserFailed(user.UserName, user.Email, result.Errors)
                };
            }

            if (roles != null && roles.Any())
            {
                var roleResult = await _userManager.AddToRolesAsync(user, roles);
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Failed to add roles to user with email {Email}: {Errors}", user.Email, roleResult.Errors.ToFormattedString());
                    var firstError = roleResult.Errors.FirstOrDefault();
                    return firstError?.Code switch
                    {
                        "UserNotFound" => AppErrors.IdentityErrors.UserNotFound(user.Id),
                        "RoleNotFound" => AppErrors.IdentityErrors.RoleNotFound(string.Join(", ", roles)),
                        "UserAlreadyInRole" => AppErrors.IdentityErrors.UserAlreadyInRole(user.Id, string.Join(", ", roles)),
                        "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(user.Id),
                        _ => AppErrors.IdentityErrors.AddToRolesFailed(user.Id, roles, roleResult.Errors)
                    };
                }
            }

            await transaction.CommitAsync();
            _logger.LogInformation("User with email {Email} registered successfully", user.Email);
            return user;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Unexpected error during user registration for email {Email}", user.Email);
            return DatabaseExceptionHandler.HandleException(ex, _logger);
        }
    }

    public async Task<ErrorOr<string>> DeleteUserByEmailAsync(string email)
    {
        _logger.LogInformation("Deleting user by email {Email}", email);

        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return AppErrors.IdentityErrors.UserEmailNotFound(email);
            }

            // Delete the user (cascade delete handles RefreshTokens)
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to delete user with email {Email}: {Errors}", email, result.Errors.ToFormattedString());
                var firstError = result.Errors.FirstOrDefault();
                return firstError?.Code switch
                {
                    "UserNotFound" => AppErrors.IdentityErrors.UserNotFound(user.Id),
                    "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(user.Id),
                    _ => AppErrors.IdentityErrors.DeleteUserFailed(user.Id, result.Errors)
                };
            }

            _logger.LogInformation("User with email {Email} deleted successfully", email);
            return email;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user deletion for email {Email}", email);
            return DatabaseExceptionHandler.HandleException(ex, _logger);
        }
    }

    public async Task<ErrorOr<User>> LoginAsync(string email, string password, bool lockoutOnFailure)
    {
        _logger.LogInformation("Attempting login for user with email {Email}", email);

        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return AppErrors.IdentityErrors.UserEmailNotFound(email);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
            if (result.Succeeded)
            {
                _logger.LogInformation("Login successful for user with email {Email}", email);
                return user;
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with email {Email} is locked out", email);
                return AppErrors.IdentityErrors.UserLockedOut(email);
            }

            if (result.IsNotAllowed)
            {
                _logger.LogWarning("User with email {Email} is not allowed to sign in", email);
                return AppErrors.IdentityErrors.UserNotAllowed(email);
            }

            if (result.RequiresTwoFactor)
            {
                _logger.LogWarning("User with email {Email} requires two-factor authentication", email);
                return AppErrors.IdentityErrors.RequiresTwoFactor(email);
            }

            _logger.LogWarning("Invalid credentials for user with email {Email}", email);
            return AppErrors.IdentityErrors.InvalidCredentials(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for email {Email}", email);
            return DatabaseExceptionHandler.HandleException(ex, _logger);
        }
    }

    public async Task<ErrorOr<User>> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating refresh token");

        try
        {
            var refreshToken = await _dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

            if (refreshToken == null)
            {
                _logger.LogWarning("Refresh token not found");
                return AppErrors.IdentityErrors.InvalidRefreshToken();
            }

            if (refreshToken.IsRevoked)
            {
                _logger.LogWarning("Refresh token is revoked");
                return AppErrors.IdentityErrors.InvalidRefreshToken();
            }

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token is expired");
                return AppErrors.IdentityErrors.InvalidRefreshToken();
            }

            return refreshToken.User;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during refresh token validation");
            return DatabaseExceptionHandler.HandleException(ex, _logger);
        }
    }

    public async Task<ErrorOr<RefreshToken>> IssueRefreshTokenAsync(string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Issuing refresh token for user {UserId}", userId);

        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return AppErrors.IdentityErrors.UserNotFound(userId);
            }

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            if (result == 0)
            {
                _logger.LogWarning("No changes were saved when issuing refresh token for user {UserId}", userId);
                return AppErrors.DatabaseErrors.NoChangesSaved();
            }

            _logger.LogInformation("Refresh token issued successfully for user {UserId}", userId);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when issuing refresh token for user {UserId}", userId);
            return DatabaseExceptionHandler.HandleException(ex, _logger);
        }
    }

    public async Task<ErrorOr<bool>> RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Revoking refresh token");

        try
        {
            var refreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

            if (refreshToken == null)
            {
                _logger.LogWarning("Refresh token not found");
                return AppErrors.IdentityErrors.InvalidRefreshToken();
            }

            refreshToken.IsRevoked = true;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            if (result == 0)
            {
                _logger.LogWarning("No changes were saved when revoking refresh token");
                return AppErrors.DatabaseErrors.NoChangesSaved();
            }

            _logger.LogInformation("Refresh token revoked successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when revoking refresh token");
            return DatabaseExceptionHandler.HandleException(ex, _logger);
        }
    }

    public async Task<ErrorOr<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching user by email {Email}", email);

        try
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("User email cannot be empty");
                return AppErrors.IdentityErrors.UserEmailCannotBeEmpty();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return AppErrors.IdentityErrors.UserEmailNotFound(email);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user fetch for email {Email}", email);
            return DatabaseExceptionHandler.HandleException(ex, _logger);
        }
    }
}

// Extension method for Identity errors
public static class IdentityResultExtensions
{
    public static string ToFormattedString(this IEnumerable<IdentityError> errors)
    {
        return string.Join("; ", errors.Select(e => $"{e.Code}: {e.Description}"));
    }
}
</xaiArtifact>

**Changes**:
- Added `GetUserByEmailAsync` to fetch a user by email using `_userManager.FindByEmailAsync`.
- Included validation for empty email and consistent error handling with `DatabaseExceptionHandler`.

#### GetUserCommandHandler.cs
Define the Command Handler to return `Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>`.

<xaiArtifact artifact_id="2e40b634-d11d-4470-9b00-1cbd26b14ef9" artifact_version_id="c36c7399-120b-41de-a05f-9012af999320" title="GetUserCommandHandler.cs" contentType="text/plain">
namespace ESLAdmin.Application.Commands;

using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Results = FluentResults.Results;

public class GetUserCommandHandler : ICommandHandler<GetUserCommand, Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>>
{
    private readonly IIdentityRepository _identityRepository;
    private readonly UserManager<User> _userManager;

    public GetUserCommandHandler(IIdentityRepository identityRepository, UserManager<User> userManager)
    {
        _identityRepository = identityRepository;
        _userManager = userManager;
    }

    public async Task<Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(GetUserCommand command, CancellationToken ct)
    {
        var result = await _identityRepository.GetUserByEmailAsync(command.Email, ct);

        if (result.IsError)
        {
            var error = result.Errors.First();
            return error.Type switch
            {
                ErrorOr.ErrorType.Validation => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = error.Code,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = error.Description
                },
                ErrorOr.ErrorType.NotFound => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = error.Code,
                    Status = StatusCodes.Status404NotFound,
                    Detail = error.Description
                },
                _ => new InternalServerError()
            };
        }

        var user = result.Value;
        var roles = await _userManager.GetRolesAsync(user);

        var response = new GetUserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Roles = roles
        };

        return Results.Ok(response);
    }
}
</xaiArtifact>

**Details**:
- Injects `IIdentityRepository` and `UserManager<User>` to fetch the user and roles.
- Calls `_identityRepository.GetUserByEmailAsync` to get the `User`.
- Maps `ErrorOr<User>` errors to `ProblemDetails` (400 for validation, 404 for not found) or `InternalServerError`.
- Fetches roles using `_userManager.GetRolesAsync`.
- Maps the `User` to `GetUserResponse` for success.
- Returns `Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>`.

#### GetUserEndpoint.cs
Define the endpoint to handle the HTTP request and execute the command.

<xaiArtifact artifact_id="4f6e95da-ad6c-45ff-a378-5423a15a8b38" artifact_version_id="a0ea54bd-08b9-4731-bf6c-12c33f7afaf4" title="GetUserEndpoint.cs" contentType="text/plain">
namespace ESLAdmin.WebApi.Endpoints;

using ESLAdmin.Application.Commands;
using ESLAdmin.WebApi.Requests;
using ESLAdmin.WebApi.Responses;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

public class GetUserEndpoint : Endpoint<GetUserRequest, GetUserResponse>
{
    public override void Configure()
    {
        Get("/api/users/{email}");
        AllowAnonymous(); // Adjust permissions as needed
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        var command = new GetUserCommand
        {
            Email = req.Email
        };

        var result = await SendAsync(command, cancellationToken: ct);

        switch (result)
        {
            case Ok<GetUserResponse> ok:
                await SendOkAsync(ok.Value, ct);
                break;
            case ProblemDetails problem:
                await SendErrorsAsync(problem.Status ?? StatusCodes.Status500InternalServerError, problem, ct);
                break;
            case InternalServerError:
                await SendErrorsAsync(StatusCodes.Status500InternalServerError, ct);
                break;
        }
    }
}
</xaiArtifact>

**Details**:
- Maps `GetUserRequest` to `GetUserCommand`.
- Executes the command using `SendAsync`.
- Handles the `Results` type, sending `200 OK` for success, `ProblemDetails` status for errors, or `500` for `InternalServerError`.

### Test the Solution
1. **Test Endpoint (Success)**:
   - Register a user:
     ```csharp
     var user = new User { UserName = "testuser", Email = "test@example.com", FirstName = "John", LastName = "Doe", PhoneNumber = "1234567890" };
     await identityRepository.RegisterUserAsync(user, "Password123!", new[] { "User" });
     ```
   - Send request:
     ```http
     GET /api/users/test@example.com HTTP/1.1
     ```
   - Verify:
     - Response: `200 OK`, `{ "Id": "user-id", "FirstName": "John", "LastName": "Doe", "UserName": "testuser", "Email": "test@example.com", "PhoneNumber": "1234567890", "Roles": ["User"] }`.
     - Logs:
       ```
       yyyy-MM-dd HH:mm:ss [Information] ESLAdmin.Infrastructure.Persistence.Repositories.IdentityRepository: Fetching user by email test@example.com
       ```

2. **Test Endpoint (User Not Found)**:
   - Send request:
     ```http
     GET /api/users/nonexistent@example.com HTTP/1.1
     ```
   - Verify:
     - Response: `404 Not Found`, `{ "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4", "title": "Identity.UserEmailNotFound", "status": 404, "detail": "User with email 'nonexistent@example.com' not found." }`.
     - Logs:
       ```
       yyyy-MM-dd HH:mm:ss [Information] ESLAdmin.Infrastructure.Persistence.Repositories.IdentityRepository: Fetching user by email nonexistent@example.com
       yyyy-MM-dd HH:mm:ss [Warning] ESLAdmin.Infrastructure.Persistence.Repositories.IdentityRepository: User with email nonexistent@example.com not found
       ```

3. **Test Endpoint (Empty Email)**:
   - Send request:
     ```http
     GET /api/users/ HTTP/1.1
     ```
   - Verify:
     - Response: `400 Bad Request`, `{ "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1", "title": "Identity.UserEmailCannotBeEmpty", "status": 400, "detail": "User email cannot be empty." }`.
     - Logs:
       ```
       yyyy-MM-dd HH:mm:ss [Information] ESLAdmin.Infrastructure.Persistence.Repositories.IdentityRepository: Fetching user by email 
       yyyy-MM-dd HH:mm:ss [Warning] ESLAdmin.Infrastructure.Persistence.Repositories.IdentityRepository: User email cannot be empty
       ```

4. **Test Endpoint (Server Error)**:
   - Simulate a database failure (e.g., by mocking `_userManager` to throw an exception).
   - Verify:
     - Response: `500 Internal Server Error`.
     - Logs:
       ```
       yyyy-MM-dd HH:mm:ss [Information] ESLAdmin.Infrastructure.Persistence.Repositories.IdentityRepository: Fetching user by email test@example.com
       yyyy-MM-dd HH:mm:ss [Error] ESLAdmin.Infrastructure.Persistence.Repositories.IdentityRepository: Unexpected error during user fetch for email test@example.com
       ```

### Additional Notes
- **Return Type**: The Command Handler returns `Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>`, as required by `ICommand`. This ensures the endpoint can handle success (`GetUserResponse`), client errors (`ProblemDetails`), and server errors (`InternalServerError`).
- **Mapping**: Manual mapping is used in `GetUserCommandHandler` for simplicity. Consider **AutoMapper** for complex mappings:
  ```csharp
  var config = new MapperConfiguration(cfg => cfg.CreateMap<User, GetUserResponse>());
  var mapper = config.CreateMapper();
  var response = mapper.Map<GetUserResponse>(user);
  ```
- **Security**: The endpoint is `AllowAnonymous` for simplicity. In production, add authentication (e.g., `[Authorize(Roles = "Admin")]`).
- **Logging**: Consistent with your pattern (`_logger.LogWarning` for validation/not found, `_logger.LogError` for exceptions).
- **Program.cs**: Ensure dependencies are registered:
  ```csharp
  builder.Services.AddFastEndpoints();
  builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();
  builder.Services.AddScoped<ICommandHandler<GetUserCommand, Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>>, GetUserCommandHandler>();
  builder.Services.AddDbContext<UserDbContext>();
  builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<UserDbContext>();
  ```

### Final Answer
- **Question**: What object/class should the `GetUserCommandHandler` return?
- **Answer**: The `GetUserCommandHandler` should return `Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>`, as specified by the `ICommand` interface. This allows:
  - Success: `Results.Ok(new GetUserResponse { ... })` with user details.
  - Client errors: `ProblemDetails` for validation (400) or not found (404).
  - Server errors: `InternalServerError` for unexpected failures (500).
- **Steps**:
  - Added `GetUserByEmailAsync` to `IdentityRepository.cs` (`artifact_id: be994219-57df-472d-806b-9189a37c1f02`).
  - Created `GetUserCommandHandler` to map `User` to `GetUserResponse` and handle errors.
  - Created `GetUserEndpoint` to execute the command and return HTTP responses.
  - Kept `AppErrors.cs` unchanged (`artifact_id: c264d344-7805-40eb-897e-8726b6e187b6`).
  - Provided tests for success, not found, validation, and server error cases.
  - Expected response for user not found:
    ```json
    {
      "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
      "title": "Identity.UserEmailNotFound",
      "status": 404,
      "detail": "User with email 'nonexistent@example.com' not found."
    }
    ```
- **Next Steps**: If you need additional endpoints, AutoMapper setup, or specific authentication/authorization logic, let me know!