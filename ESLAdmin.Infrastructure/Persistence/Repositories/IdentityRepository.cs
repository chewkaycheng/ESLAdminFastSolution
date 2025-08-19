using Dapper;
using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Domain.Dtos;
using ESLAdmin.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using ESLAdmin.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence.Identity;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Extensions;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace ESLAdmin.Infrastructure.Persistence.Repositories;

//------------------------------------------------------------------------------
//
//                       class IIdentityRepository
//
//-------------------------------------------------------------------------------
public class IdentityRepository : IIdentityRepository
{
  private readonly IMessageLogger _messageLogger;
  private readonly ILogger _logger;
  private readonly UserManager<User> _userManager;
  private readonly SignInManager<User> _signInManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly UserDbContext _dbContext;
  private readonly IDbContextDapper _dbContextDapper;

  //------------------------------------------------------------------------------
  //
  //                       IdentityRepository
  //
  //-------------------------------------------------------------------------------
  public IdentityRepository(
    ILogger logger,
    IMessageLogger messageLogger,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<IdentityRole> roleManager,
    UserDbContext dbContext,
    IDbContextDapper dbContextDapper)
  {
    _dbContext = dbContext;
    _logger = logger;
    _messageLogger = messageLogger;
    _userManager = userManager;
    _signInManager = signInManager;
    _roleManager = roleManager;
    _dbContextDapper = dbContextDapper;
  }

  //------------------------------------------------------------------------------
  //
  //                       AddToRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> AddToRoleAsync(
    string email,
    string roleName)
  {
    _logger.LogFunctionEntry(
      $"Adding user: '{email}' to role: '{roleName}'.");

    try
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        _logger.LogWarning("User is null");
        return AppErrors.IdentityErrors.UserEmailNotFound(email);
      }

      //var role = await _roleManager.FindByNameAsync(roleName);
      //if (role == null)
      //{
      //  return Errors.IdentityErrors.RoleNotFound(roleName);
      //}

      var result = await _userManager.AddToRoleAsync(user, roleName);

      if (!result.Succeeded)
      {

        // Errors: 
        // 1) UserNotFound
        // 2) RoleNotFound
        // 3) UserAlreadyInRole
        // 4) ConcurrencyFailure
        // 5) InvalidRoleName
        _logger.LogIdentityErrors(
          "_userManager.AddToRoleAsync", 
          email, 
          result.Errors.ToFormattedString());

        var firstError = result.Errors.FirstOrDefault();
        return firstError.Code switch
        {
          "ConcurrencyFailure" => AppErrors.DatabaseErrors.ConcurrencyFailure(),
          _ => AppErrors.IdentityErrors.AddToRoleFailed()
        };
      }

      _logger.LogFunctionExit();
      return new ErrorOr.Success();
    }
    catch (Exception ex)
    {
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       DeleteUserByEmailAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> DeleteUserByEmailAsync(string email)
  {
    _logger.LogFunctionEntry($"Deleting user by email: '{email}'.");

    try
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        _logger.LogWarning($"User with email '{email}' not found.");
        return AppErrors.IdentityErrors.UserEmailNotFound(email);
      }
      
      // Optional: Check and remove roles explicity (usually not needed due to cascade delete)
      //var roles = await _userManager.GetRolesAsync(user);
      //if (roles.Any())
      //{
      //  var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
      //  if (!removeRolesResult.Succeeded)
      //  {
      //    InfoLogIdentityErrors("GetRolesAsync", email, removeRolesResult.Errors);
      //    return IdentityResultEx.Failed(IdentityErrorTypes.RemoveFromRolesError, removeRolesResult.Errors.ToArray());
      //  }
      //}

      // Delete the user

      var result = await _userManager.DeleteAsync(user);
      if (!result.Succeeded)
      {
        _logger.LogIdentityErrors(
         "_userManager.DeleteAsync",
         email,
         result.Errors.ToFormattedString());

        var firstError = result.Errors.FirstOrDefault();
        return firstError.Code switch
        {
          "UserNotFound" => AppErrors.IdentityErrors.DeleteUserFailed(),
          "ConcurrencyFailure" => AppErrors.DatabaseErrors.ConcurrencyFailure()
        };
      }

      _logger.LogFunctionExit($"User with email '{email}' deleted successfully.");
      return new ErrorOr.Success();
    }
    catch (Exception ex)
    {
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetUserByEmailAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<User>> GetUserByEmailAsync(
    string email)
  {
    _logger.LogFunctionEntry($"Get user by email: '{email}'.");
    try
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user == null)
      {
        _logger.LogWarning("User is null");
        return AppErrors.IdentityErrors.UserEmailNotFound(email);
      }

      _logger.LogFunctionExit();
      return user;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetRolesForUserAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<List<string>>> GetRolesForUserAsync(
    User user)
  {
    _logger.LogFunctionEntry($"Get roles for by user: '{user.Email}'.");
    try
    {
      if (user == null)
      {
        _logger.LogError("User is null.");
        return AppErrors.IdentityErrors.InvalidUser("User cannot be null.");
      }

      var roles =  await _userManager.GetRolesAsync(user);
      _logger.LogFunctionExit();                  
      return roles.ToList();
    }
    catch (Exception ex)
    {
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetAllUserRolesAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<IEnumerable<UserRole>>> GetAllUserRolesAsync()
  {
    try
    {
      _logger.LogFunctionEntry("Get all user roles");

      var connectionResult = await _dbContextDapper.GetConnectionAsync();
      if (connectionResult.IsError)
      {
        _logger.LogError(
          "Failed to get database connection: {Error}", 
          connectionResult.Errors.ToFormattedString());
        return AppErrors.DatabaseErrors.DatabaseConnectionError(
          "Failed to get database connection");
      }

      using IDbConnection connection = connectionResult.Value;
      var userRoles = await connection.QueryAsync<UserRole>(
        DbConstsIdentity.SQL_GETUSERROLES,
        commandType: CommandType.Text);

      _logger.LogFunctionExit();
      return userRoles.ToList();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetAllUsersAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<IEnumerable<User>>> GetAllUsersAsync()
  {
    try
    {
      _logger.LogFunctionEntry("Get all users");
      var sql = DbConstsIdentity.SQL_GETALL;

      var connectionResult = await _dbContextDapper.GetConnectionAsync();
      if (connectionResult.IsError)
      {
        _logger.LogError(
          "Failed to get database connection: {Error}", 
          connectionResult.Errors);
        return AppErrors.DatabaseErrors.DatabaseConnectionError(
          "Failed to get database connection");
      }

      using IDbConnection connection = connectionResult.Value;
      var users = await connection.QueryAsync<User>(
        sql,
        commandType: CommandType.Text);

      _logger.LogFunctionExit();
      return users.ToList();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
         .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       LoginAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<User>> LoginAsync(string email, string password)
  {
    try
    {
      _logger.LogFunctionEntry();

      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        _logger.LogWarning("User is null.");
        return AppErrors.IdentityErrors.InvalidCredentials(email);
      }

      var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

      if (result.Succeeded)
      {
        _logger.LogFunctionExit();
        return user;
      }
      else
      {
        _logger.AuthenticationFailure(
                      user.Id,
                      result.IsLockedOut,
                      result.IsNotAllowed,
                      result.RequiresTwoFactor);

        if (result.IsLockedOut)
        {
          _logger.LogWarning("User with email {Email} is locked out", email);
          return AppErrors.IdentityErrors.UserLockedOut(email);
        }

        if (result.IsNotAllowed)
        {
          _logger.LogWarning("User with email {Email} is not allowed to sign in", email);
          return AppErrors.IdentityErrors.IsNotAllowed(email);
        }

        if (result.RequiresTwoFactor)
        {
          _logger.LogWarning("User with email {Email} requires two-factor authentication", email);
          return AppErrors.IdentityErrors.RequiresTwoFactor(email);
        }

        _logger.LogWarning("Invalid credentials for user with email {Email}", email);
        return AppErrors.IdentityErrors.InvalidCredentials(email);
      }
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
         .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       ValidateRefreshTokenAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<RefreshToken>> ValidateRefreshTokenAsync(
    string token,
    CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogFunctionEntry();

      var refreshToken = await _dbContext.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.Token == token,
          cancellationToken);

      if (refreshToken == null)
      {
        _logger.LogWarning("Refresh token not found.");
        return AppErrors.IdentityErrors.RefreshTokenNotFound(token);
      }

      if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
      {
        _logger.LogWarning($"Refresh token is revoked or expired for user '{refreshToken.UserId}'.");
        return AppErrors.IdentityErrors.RefreshTokenInvalid("Refresh token is revoked or expired.");
      }

      var user = await _userManager.FindByIdAsync(refreshToken.UserId);
      if (user == null)
      {
        _logger.LogWarning($"User: '{refreshToken.UserId}' not found for refresh token.");
        return AppErrors.IdentityErrors.UserIdNotFound(refreshToken.UserId);
      }

      _logger.LogFunctionExit();
      return new RefreshToken
      {
        UserId = refreshToken.UserId,
        Token = refreshToken.Token,
        ExpiresAt = refreshToken.ExpiresAt
      };
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
           .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GenerateRefreshTokenAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<string>> GenerateRefreshTokenAsync(
    string userId,
    CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogFunctionEntry();

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        _logger.LogWarning($"User: '{userId}' not found.");
        return AppErrors.IdentityErrors.UserIdNotFound(userId);
      }

      var token = Guid.NewGuid().ToString();
      var expiryDate = DateTime.UtcNow.AddDays(30);

      var refreshToken = new RefreshToken
      {
        UserId = userId,
        Token = token,
        ExpiresAt = expiryDate,
        IsRevoked = false
      };

      _dbContext.RefreshTokens.Add(refreshToken);
      await _dbContext.SaveChangesAsync(cancellationToken);

      _logger.LogFunctionExit("Generated refresh token for user '{userId}'.");
      return token;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetRefreshTokenAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<RefreshToken>> GetRefreshTokenAsync(string token)
  {
    try
    {
      _logger.LogFunctionEntry();
      var refreshToken = await _dbContext.RefreshTokens
        .FirstOrDefaultAsync(
          rt => rt.Token == token &&
          !rt.IsRevoked &&
          rt.ExpiresAt > DateTime.UtcNow);
      if (refreshToken == null)
      {
        _logger.LogWarning($"Refresh token: '{token}' is null.");
        return AppErrors.IdentityErrors.RefreshTokenNotFound(token);
      }
      _logger.LogFunctionExit();
      return refreshToken;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       AddRefreshTokenAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> AddRefreshTokenAsync(RefreshToken refreshToken)
  {
    try
    {
      _logger.LogFunctionEntry();
      await _dbContext.RefreshTokens.AddAsync(refreshToken);
      await _dbContext.SaveChangesAsync();
      _logger.LogFunctionExit();
      return new Success();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       RevokeRefreshTokenAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> RevokeRefreshTokenAsync(string token)
  {
    try
    {
      _logger.LogFunctionEntry();
      var refreshToken = await _dbContext
                                 .RefreshTokens
                                 .FirstOrDefaultAsync(
                                   rt => rt.Token == token);
      if (refreshToken == null)
      {
        return AppErrors.IdentityErrors.InvalidToken();
      }
      refreshToken.IsRevoked = true;
      await _dbContext.SaveChangesAsync();
      _logger.LogFunctionExit();
      return new Success();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       RevokeRefreshTokenAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> RevokeRefreshTokenAsync(
    string userId,
    CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogFunctionEntry();
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        _logger.LogWarning($"User: '{userId}' not found.");
        return AppErrors.IdentityErrors.UserIdNotFound(userId);
      }

      var tokens = await _dbContext.RefreshTokens
        .Where(rt => rt.UserId == userId && !rt.IsRevoked)
        .ToListAsync(cancellationToken);

      if (!tokens.Any())
      {
        _logger.LogWarning($"No active refresh tokens found for user: '{userId}'.");
        return Result.Success;
      }

      foreach (var token in tokens)
      {
        token.IsRevoked = true;
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
      _logger.LogInformation($"Revoked {tokens.Count} refresh tokens for user: '{userId}'.");
      return Result.Success;
    }
    catch (DbUpdateException ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.TokenRevocationFailed(userId, ex.Message);
    }
    catch (OperationCanceledException ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.OperationCanceled();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.TokenRevocationFailed(userId, ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       RegisterUserAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> RegisterUserAsync(
    User user,
    string password,
    ICollection<string>? roles)
  {
    _logger.LogFunctionEntry($"Registering user with email: '{user.Email}'");

    // Check for any invalid roles before doing transaction
    if (roles != null && roles.Any())
    {
      var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
      var invalidRoles = roles.Except(allRoles).ToList();

      if (invalidRoles != null && invalidRoles.Count() > 0)
      {
        _logger.LogWarning($"Invalid roles provided: {string.Join(", ", invalidRoles)}.");
        return AppErrors.IdentityErrors.InvalidRoles(invalidRoles);
      }
    }

    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
      var result = await _userManager.CreateAsync(
        user,
        password);

      if (!result.Succeeded)
      {
        await transaction.RollbackAsync();
        _logger.LogError(
          $"Failed to create user with email '{user.Email}': {result.Errors.ToFormattedString()}");

        // Errors:
        // 1) DuplicateUserName
        // 2) DuplicateEmail
        // 3) InvalidUserName
        // 4) InvalidEmail
        // 5) PasswordTooShort
        // 6) PasswordRequiresNonAlphanumeric
        // 7) PasswordRequiresDigit
        // 8) PasswordRequiresLower
        // 9) PasswordRequiresUpper
        return IdentityErrorHandler.HandleIdentityErrors(
          result,
          _logger,
          IdentityOperation.CreateUser,
          userId: user.Id,
          userName: user.UserName ?? "",
          email: user.Email);
      }

      if (roles != null && roles.Any())
      {
        var roleResult = await _userManager.AddToRolesAsync(user, roles);
        if (!roleResult.Succeeded)
        {
          await transaction.RollbackAsync();

          _logger.LogError(
            $"Failed to add roles to user with email '{user.Email}': {roleResult.Errors.ToFormattedString()}");

          // Errors
          // 1) UserNotFound
          // 2) RoleNotFound
          // 3) UserAlreadyInRole
          // 4) InvalidRoleName
          // 5) ConcurrencyFailure
          var firstError = result.Errors.FirstOrDefault();
          return firstError?.Code switch
          {
            "UserNotFound" => AppErrors.IdentityErrors.AddToRolesError(),
            "RoleNotFound" => AppErrors.IdentityErrors.AddToRolesError(),
            "UserAlreadyInRole" => AppErrors.IdentityErrors.AddToRolesError(),
            "InvalidRoleName" => AppErrors.IdentityErrors.AddToRolesError(),
            "ConcurrencyFailure" => AppErrors.DatabaseErrors.ConcurrencyFailure(),
          };
        }
      }

      await transaction.CommitAsync();
      _logger.LogFunctionExit($"User with email '{user.Email}' registered successfully.");
      return new Success();
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      _logger.LogException(ex);
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       RemoveFromRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<string>> RemoveFromRoleAsync(
    string email,
    string roleName)
  {
    try
    {
      _logger.LogFunctionEntry();

      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return AppErrors.IdentityErrors.UserEmailNotFound(email);
      }

      var role = await _roleManager.FindByNameAsync(roleName);
      if (role == null)
      {
        return AppErrors.IdentityErrors.RoleNotFound(roleName);
      }

      var result = await _userManager.RemoveFromRoleAsync(user, roleName);
      if (!result.Succeeded)
      {
        _logger.LogIdentityErrors(
          "_userManager.RemoveFromRoleAsync",
          email,
          result.Errors.ToFormattedString());

        return IdentityErrorHandler.HandleIdentityErrors(
          result,
          _logger,
          IdentityOperation.RemoveFromRole,
          userId: user.Id,
          roleName: roleName);
      }

      _logger.LogFunctionExit();
      return roleName;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);

    }
  }

  //------------------------------------------------------------------------------
  //
  //                       FindByIdAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<User>> FindByIdAsync(string userId)
  {
    try
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        return AppErrors.IdentityErrors.UserIdNotFound(userId);
      }
      return user;
    }
    catch (Exception ex)
    {
      return DatabaseExceptionHandler
          .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       FindByUserNameAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<User>> FindByUserNameAsync(string username)
  {
    var user = await _userManager.FindByNameAsync(username);
    if (user == null)
    {
      return AppErrors.IdentityErrors.UserNameNotFound(username);
    }
    return user;
  }

  //------------------------------------------------------------------------------
  //
  //                       CreateRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<IdentityRole>> CreateRoleAsync(string roleName)
  {
    //if (await _roleManager.RoleExistsAsync(roleName))
    //{
    //  return Errors.IdentityErrors.RoleExists(roleName);
    //}

    var role = new IdentityRole(roleName);
    var result = await _roleManager.CreateAsync(role);

    if (!result.Succeeded)
    {
      _logger.LogIdentityErrors("_roleManager.CreateAsync", roleName, result.Errors.ToFormattedString());

      return IdentityErrorHandler.HandleIdentityErrors(
        result,
        _logger,
        IdentityOperation.RemoveFromRole,
        roleName: roleName);
    }

    return role;
  }

  //------------------------------------------------------------------------------
  //
  //                       DeleteRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<string>> DeleteRoleAsync(string roleName)
  {

    var role = await _roleManager.FindByNameAsync(roleName);
    if (role == null)
    {
      return AppErrors.IdentityErrors.RoleNotFound(roleName);
    }

    var result = await _roleManager.DeleteAsync(role);
    if (!result.Succeeded)
    {
      _logger.LogIdentityErrors(
        "_roleManager.DeleteAsync", 
        "roleName", 
        result.Errors.ToFormattedString());

      return IdentityErrorHandler.HandleIdentityErrors(
        result,
        _logger,
        IdentityOperation.DeleteRole,
        roleName: roleName);
    }

    return role.Id;
  }

  //------------------------------------------------------------------------------
  //
  //                       GetRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<List<string>>> GetRolesAsync(User user)
  {
    try
    {
      _logger.LogFunctionExit();
      IList<string> roles = await _userManager.GetRolesAsync(user);
      _logger.LogFunctionExit();
      return roles.ToList();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
        .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       UpdateRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<string>> UpdateRoleAsync(string oldRoleName, string newRoleName)
  {
    var role = await _roleManager.FindByNameAsync(oldRoleName);
    if (role == null)
    {
      return AppErrors.IdentityErrors.RoleNotFound(oldRoleName);
    }

    //if (await _roleManager.RoleExistsAsync(newRoleName))
    //{
    //  return Errors.IdentityErrors.RoleExists(newRoleName);
    //}

    role.Name = newRoleName;
    var result = await _roleManager.UpdateAsync(role);
    if (!result.Succeeded)
    {
      _logger.LogIdentityErrors(
        "_roleManager.UpdateAsync", 
        newRoleName, 
        result.Errors.ToFormattedString());

      return IdentityErrorHandler.HandleIdentityErrors(
        result,
        _logger,
        IdentityOperation.DeleteRole,
        roleName: role.Name,
        oldRoleName: oldRoleName,
        newRoleName: newRoleName);
    }

    return newRoleName;
  }

  //------------------------------------------------------------------------------
  //
  //                       GetRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<IdentityRole>> GetRoleAsync(string roleName)
  {
    var role = await _roleManager.FindByNameAsync(roleName);
    if (role == null)
    {
      return AppErrors.IdentityErrors.RoleNotFound(roleName);
    }
    return role;
  }

  //------------------------------------------------------------------------------
  //
  //                       GetAllRolesAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<IEnumerable<IdentityRole>>> GetAllRolesAsync()
  {
    try
    {
      return await _roleManager.Roles.ToListAsync();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return AppErrors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       InfoLogIdentityErrors
  //
  //-------------------------------------------------------------------------------
  private void InfoLogIdentityErrors(
    string identityFunction,
    string id,
    IEnumerable<IdentityError> errors)
  {
    if (_logger.IsEnabled(LogLevel.Information))
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("  Errors:");
      foreach (var error in errors)
      {
        sb.AppendLine($"    Code: {error.Code}\tDescription: {error.Description}");
      }
      _logger.LogIdentityErrors(identityFunction, id, sb.ToString());
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       MapToDto
  //
  //-------------------------------------------------------------------------------
  private UserDto MapToDto(User user, IList<string> roles)
  {
    UserDto userDto = new UserDto
    {
      Id = user.Id,
      FirstName = user.FirstName,
      LastName = user.LastName,
      UserName = user.UserName,
      Email = user.Email,
      PhoneNumber = user.PhoneNumber,
      Roles = roles == null ? new List<string>() :
        roles.ToList()
    };
    return userDto;
  }
}

