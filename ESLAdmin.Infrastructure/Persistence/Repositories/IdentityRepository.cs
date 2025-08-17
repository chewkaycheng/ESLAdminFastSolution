using Dapper;
using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Domain.Dtos;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using ESLAdmin.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Extensions;
using ESLAdmin.Logging.Interface;
using FirebirdSql.Data.FirebirdClient;
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
  public async Task<ErrorOr<string>> AddToRoleAsync(
    string email,
    string roleName)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
      return AppErrors.IdentityErrors.UserEmailNotFound(email);
    }

    //var role = await _roleManager.FindByNameAsync(roleName);
    //if (role == null)
    //{
    //  return Errors.IdentityErrors.RoleNotFound(roleName);
    //}

    try
    {
      _logger.LogFunctionEntry($"Adding user: '{user.Id}' to role: '{roleName}'.");

      var result = await _userManager.AddToRoleAsync(user, roleName);

      if (!result.Succeeded)
      {
        _logger.LogIdentityErrors("_userManager.AddToRoleAsync", email, result.Errors.ToFormattedString());
        var firstError = result.Errors.FirstOrDefault();
        return firstError?.Code switch
        {
          "UserNotFound" => AppErrors.IdentityErrors.UserNotFound(user.Id),
          //"RoleNotFound" => Errors.IdentityErrors.RoleNotFound(roleName),
          "UserAlreadyInRole" => AppErrors.IdentityErrors.UserAlreadyInRole(user.Id, roleName),
          "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(user.Id),
          _ => AppErrors.IdentityErrors.AddToRoleFailed(user.Id, roleName, result.Errors)
        };
      }

      _logger.LogFunctionExit();
      return roleName;
    }
    catch (ArgumentNullException ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.InvalidArgument(ex.Message);
    }
    catch (FbException ex)
    {
      _logger.LogException(ex);
      return AppErrors.DatabaseErrors.DatabaseError($"Firebird error: {ex.Message} (ErrorCode: {ex.ErrorCode})");
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.InvalidOperation(ex.Message);
    }
    catch (OperationCanceledException ex)
    {
      _logger.LogException(ex);
      return AppErrors.DatabaseErrors.OperationCanceled();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return AppErrors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       DeleteUserByEmailAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<string>> DeleteUserByEmailAsync(string email)
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
        _logger.LogError($"Failed to delete user with email '{email}': {result.Errors.ToFormattedString()}");
        var firstError = result.Errors.FirstOrDefault();
        return firstError?.Code switch
        {
          "UserNotFound" => AppErrors.IdentityErrors.UserNotFound(user.Id),
          "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(user.Id),
          _ => AppErrors.IdentityErrors.DeleteUserFailed(user.Id, result.Errors)
        };
      }

      _logger.LogFunctionExit($"User with email '{email}' deleted successfully.");
      return email;
    }
    catch (ArgumentNullException ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.InvalidArgument(ex.Message);
    }
    catch (DbUpdateException ex)
    {
      _logger.LogException(ex);
      return AppErrors.DatabaseErrors.DatabaseError(ex.InnerException?.Message ?? ex.Message);
    }
    catch (OperationCanceledException ex)
    {
      _logger.LogException(ex);
      return AppErrors.DatabaseErrors.OperationCanceled();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return AppErrors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetUserByEmailAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<UserDto>> GetUserByEmailAsync(
    string email)
  {
    var user = await _userManager.FindByEmailAsync(email);

    if (user == null)
    {
      return AppErrors.IdentityErrors.UserEmailNotFound(email);
    }

    var roles = await _userManager.GetRolesAsync(user);
    return MapToDto(user, roles);
  }

  //------------------------------------------------------------------------------
  //
  //                       GetAllUsersAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
  {
    var sql = DbConstsIdentity.SQL_GETALL;

    var connectionResult = await _dbContextDapper.GetConnectionAsync();
    if (connectionResult.IsError)
    {
      _logger.LogError("Failed to get database connection: {Error}", connectionResult.Errors);
      return Enumerable.Empty<UserDto>();
    }

    using IDbConnection connection = connectionResult.Value;
    var users = await connection.QueryAsync<User>(
      sql,
      commandType: CommandType.Text);

    var userRoles = await connection.QueryAsync<UserRole>(
      DbConstsIdentity.SQL_GETUSERROLES,
      commandType: CommandType.Text);

    var userDtos = new List<UserDto>();
    foreach (var user in users)
    {
      var userDto = new UserDto
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        UserName = user.UserName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Roles = userRoles == null ? new List<string>() :
          userRoles.Where(r => r.UserId == user.Id)
          .Select(r => r.Name)
          .ToList()
      };
      userDtos.Add(userDto);
    }

    return userDtos;
  }

  //------------------------------------------------------------------------------
  //
  //                       LoginAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<UserDto>> LoginAsync(string email, string password)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
      return AppErrors.IdentityErrors.UserLoginFailed();
    }

    var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
    if (!result.Succeeded)
    {
      _logger.LogWarning(
                    "Sign-in failed for user '{UserId}': IsLockedOut={IsLockedOut}, IsNotAllowed={IsNotAllowed}, RequiresTwoFactor={RequiresTwoFactor}",
                    user.Id,
                    result.IsLockedOut,
                    result.IsNotAllowed,
                    result.RequiresTwoFactor);

      return AppErrors.IdentityErrors.UserLoginFailed();
    }

    var roles = await _userManager.GetRolesAsync(user);
    return MapToDto(user, roles);
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
      var refreshToken = await _dbContext.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.Token == token,
          cancellationToken);

      if (refreshToken == null)
      {
        _logger.LogCustomError("Refresh token not found.");
        return AppErrors.IdentityErrors.RefreshTokenNotFound(token);
      }

      if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
      {
        _logger.LogCustomError($"Refresh token is revoked or expired for user '{refreshToken.UserId}'.");
        return AppErrors.IdentityErrors.RefreshTokenInvalid("Refresh token is revoked or expired.");
      }

      var user = await _userManager.FindByIdAsync(refreshToken.UserId);
      if (user == null)
      {
        _logger.LogCustomError($"User: '{refreshToken.UserId}' not found for refresh token.");
        return AppErrors.IdentityErrors.UserIdNotFound(refreshToken.UserId);
      }

      return new RefreshToken
      {
        UserId = refreshToken.UserId,
        Token = refreshToken.Token,
        ExpiresAt = refreshToken.ExpiresAt
      };
    }
    catch (OperationCanceledException ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.OperationCanceled();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.RefreshTokenInvalid($"Unexpected error: {ex.Message}");
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
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        _logger.LogCustomError("User: '{userId}' not found.");
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

      _logger.LogCustomInformation("Generated refresh token for user '{userId}'.");
      return token;
    }
    catch (DbUpdateException ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.TokenGenerationFailed(userId, ex.Message);
    }
    catch (OperationCanceledException ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.OperationCanceled();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return AppErrors.IdentityErrors.TokenGenerationFailed(userId, ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetRefreshTokenAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<RefreshToken>> GetRefreshTokenAsync(string token)
  {
    var refreshToken = await _dbContext.RefreshTokens
      .FirstOrDefaultAsync(
        rt => rt.Token == token &&
        !rt.IsRevoked &&
        rt.ExpiresAt > DateTime.UtcNow);
    if (refreshToken == null)
    {
      return AppErrors.IdentityErrors.RefreshTokenNotFound(token);
    }
    return refreshToken;
  }

  //------------------------------------------------------------------------------
  //
  //                       AddRefreshTokenAsync
  //
  //-------------------------------------------------------------------------------
  public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
  {
    await _dbContext.RefreshTokens.AddAsync(refreshToken);
    await _dbContext.SaveChangesAsync();
  }

  //------------------------------------------------------------------------------
  //
  //                       RevokeRefreshTokenAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<bool>> RevokeRefreshTokenAsync(string token)
  {
    var refreshToken = await _dbContext
                               .RefreshTokens
                               .FirstOrDefaultAsync(
                                 rt => rt.Token == token);
    if (refreshToken == null)
    {
      return AppErrors.IdentityErrors.RefreshTokenNotFound(token);
    }
    refreshToken.IsRevoked = true;
    await _dbContext.SaveChangesAsync();
    return true;
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
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        _logger.LogCustomError($"User: '{userId}' not found.");
        return AppErrors.IdentityErrors.UserIdNotFound(userId);
      }

      var tokens = await _dbContext.RefreshTokens
        .Where(rt => rt.UserId == userId && !rt.IsRevoked)
        .ToListAsync(cancellationToken);

      if (!tokens.Any())
      {
        _logger.LogCustomInformation($"No active refresh tokens found for user: '{userId}'.");
        return Result.Success;
      }

      foreach (var token in tokens)
      {
        token.IsRevoked = true;
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
      _logger.LogCustomInformation($"Revoked {tokens.Count} refresh tokens for user: '{userId}'.");
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
  public async Task<ErrorOr<User>> RegisterUserAsync(
    User user,
    string password,
    ICollection<string>? roles)
  {
    _logger.LogInformation($"Registering user with email: '{user.Email}'");

    if (string.IsNullOrEmpty(user.Email))
    {
      _logger.LogWarning("User email cannot be empty.");
      return AppErrors.IdentityErrors.UserEmailCannotBeEmpty();
    }

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

          _logger.LogError(
            $"Failed to add roles to user with email '{user.Email}': {roleResult.Errors.ToFormattedString()}");

          var firstError = result.Errors.FirstOrDefault();
          return firstError?.Code switch
          {
            "UserNotFound" => AppErrors.IdentityErrors.UserNotFound(user.Id),
            "RoleNotFound" => AppErrors.IdentityErrors.RoleNotFound(string.Join(", ", roles)),
            "UserAlreadyInRole" => AppErrors.IdentityErrors.UserAlreadyInRole(user.Id, string.Join(", ", roles)),
            "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(user.Id),
            _ => AppErrors.IdentityErrors.AddToRolesFailed(user.Id, roles, result.Errors)
          };
        }
      }

      await transaction.CommitAsync();
      _logger.LogFunctionExit($"User with email '{user.Email}' registered successfully.");
      return user;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      _logger.LogException(ex);
      return AppErrors.CommonErrors.Exception(ex.Message);
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
      _logger.LogIdentityErrors("_userManager.AddToRoleAsync", email, result.Errors.ToFormattedString());
      var firstError = result.Errors.FirstOrDefault();
      return firstError?.Code switch
      {
        "UserNotFound" => AppErrors.IdentityErrors.UserNotFound(user.Id),
        "RoleNotFound" => AppErrors.IdentityErrors.RoleNotFound(roleName),
        "UserNotInRole" => AppErrors.IdentityErrors.UserNotInRole(user.Id, roleName),
        "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(user.Id),
        _ => AppErrors.IdentityErrors.RemoveFromRoleFailed(user.Id, roleName, result.Errors)
      };
    }

    return roleName;
  }

  //------------------------------------------------------------------------------
  //
  //                       FindByIdAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<User>> FindByIdAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return AppErrors.IdentityErrors.UserIdNotFound(userId);
    }
    return user;
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

      // Map specific Identity errors to ErrorOr
      var firstError = result.Errors.FirstOrDefault();
      return firstError?.Code switch
      {
        "DuplicateRoleName" => AppErrors.IdentityErrors.DuplicateRoleName(roleName),
        "InvalidRoleName" => AppErrors.IdentityErrors.InvalidRoleName(roleName),
        "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(roleName),
        _ => AppErrors.IdentityErrors.CreateRoleFailed(roleName, result.Errors)
      };
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
      _logger.LogIdentityErrors("_roleManager.DeleteAsync", "roleName", result.Errors.ToFormattedString());
      var firstError = result.Errors.FirstOrDefault();
      return firstError?.Code switch
      {
        "RoleNotFound" => AppErrors.IdentityErrors.RoleNotFound(roleName),
        "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(roleName),
        _ => AppErrors.IdentityErrors.DeleteRoleFailed(roleName, result.Errors)
      };
    }

    return role.Id;
  }

  //------------------------------------------------------------------------------
  //
  //                       GetRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<List<string>> GetRolesAsync(User user)
  {
    IList<string> roles = await _userManager.GetRolesAsync(user);
    return roles.ToList();
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
      _logger.LogIdentityErrors("_roleManager.UpdateAsync", newRoleName, result.Errors.ToFormattedString());
      var firstError = result.Errors.First();
      return firstError.Code switch
      {
        "RoleNotFound" => AppErrors.IdentityErrors.RoleNotFound(role.Name),
        "DuplicateRoleName" => AppErrors.IdentityErrors.DuplicateRoleName(role.Name),
        "InvalidRoleName" => AppErrors.IdentityErrors.InvalidRoleName(role.Name),
        "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(role.Name),
        _ => AppErrors.IdentityErrors.UpdateRoleFailed(oldRoleName, newRoleName, result.Errors)
      };
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

