using Dapper;
using ErrorOr;
using ESLAdmin.Common.Errors;
using ESLAdmin.Domain.Dtos;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.Data;
using ESLAdmin.Infrastructure.Data.Consts;
using ESLAdmin.Infrastructure.Data.Interfaces;
using ESLAdmin.Infrastructure.Repositories.Interfaces;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Extensions;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace ESLAdmin.Features.Users.Repositories;

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
      return Errors.IdentityErrors.UserEmailNotFound(email);
    }

    //var role = await _roleManager.FindByNameAsync(roleName);
    //if (role == null)
    //{
    //  return Errors.IdentityErrors.RoleNotFound(roleName);
    //}

    var result = await _userManager.AddToRoleAsync(user, roleName);

    if (!result.Succeeded)
    {
      _logger.LogIdentityErrors("_userManager.AddToRoleAsync", email, result.Errors.ToFormattedString());
      var firstError = result.Errors.FirstOrDefault();
      return firstError?.Code switch
      {
        "UserNotFound" => Errors.IdentityErrors.UserNotFound(user.Id),
        "RoleNotFound" => Errors.IdentityErrors.RoleNotFound(roleName),
        "UserAlreadyInRole" => Errors.IdentityErrors.UserAlreadyInRole(user.Id, roleName),
        "ConcurrencyFailure" => Errors.IdentityErrors.ConcurrencyFailure(user.Id),
        _ => Errors.IdentityErrors.AddToRoleFailed(user.Id, roleName, result.Errors)
      };
    }

    return roleName;
  }

  //------------------------------------------------------------------------------
  //
  //                       DeleteUserByEmailAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<string>> DeleteUserByEmailAsync(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
      return Errors.IdentityErrors.UserEmailNotFound(email);
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
      _logger.LogIdentityErrors("_userManager.DeleteAsync", email, result.Errors.ToFormattedString());
      var firstError = result.Errors.FirstOrDefault();
      return firstError?.Code switch
      {
        "UserNotFound" => Errors.IdentityErrors.UserNotFound(user.Id),
        "ConcurrencyFailure" => Errors.IdentityErrors.ConcurrencyFailure(user.Id),
        _ => Errors.IdentityErrors.DeleteUserFailed(user.Id, result.Errors)
      };
    }

    return email;
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
      return Errors.IdentityErrors.UserEmailNotFound(email);
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
          (IList<string>)userRoles.Where(r => r.UserId == user.Id)
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
      return Errors.IdentityErrors.UserLoginFailed();
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

      return Errors.IdentityErrors.UserLoginFailed();
    }

    var roles = await _userManager.GetRolesAsync(user);
    return MapToDto(user, roles);
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
      return Errors.IdentityErrors.RefreshTokenNotFound(token);
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
      return Errors.IdentityErrors.RefreshTokenNotFound(token);
    }
    refreshToken.IsRevoked = true;
    await _dbContext.SaveChangesAsync();
    return true;
  }

  //------------------------------------------------------------------------------
  //
  //                       RegisterUser
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<User>> RegisterUserAsync(
    User user,
    string password,
    ICollection<string>? roles)
  {
    if (string.IsNullOrEmpty(user.Email))
    {
      return Errors.IdentityErrors.UserEmailCannotBeEmpty();
    }

    // Check for any invalid roles before doing transaction
    if (roles != null && roles.Any())
    {
      var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
      var invalidRoles = roles.Except(allRoles).ToList();

      if (invalidRoles != null && invalidRoles.Count() > 0)
        return Errors.IdentityErrors.InvalidRoles(invalidRoles);
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

        _logger.LogIdentityErrors(
          "_userManager.CreateAsync",
          user.Email,
          result.Errors.ToFormattedString());

        var firstError = result.Errors.FirstOrDefault();
        return firstError?.Code switch
        {
          "DuplicateUserName" => Errors.IdentityErrors.DuplicateUserName(user.UserName),
          "DuplicateEmail" => Errors.IdentityErrors.DuplicateEmail(user.Email),
          "InvalidUserName" => Errors.IdentityErrors.InvalidUserName(user.UserName),
          "InvalidEmail" => Errors.IdentityErrors.InvalidEmail(user.Email ?? "null"),
          "ConcurrencyFailure" => Errors.IdentityErrors.ConcurrencyFailure(user.UserName),
          _ => Errors.IdentityErrors.CreateUserFailed(user.UserName, user.Email, result.Errors)
        };
      }

      if (roles != null && roles.Any())
      {
        var roleResult = await _userManager.AddToRolesAsync(user, roles);
        if (!roleResult.Succeeded)
        {
          await transaction.RollbackAsync();

          _logger.LogIdentityErrors(
            "_userManager.AddToRolesAsync",
            user.Email,
            result.Errors.ToFormattedString());

          var firstError = result.Errors.FirstOrDefault();
          return firstError?.Code switch
          {
            "UserNotFound" => Errors.IdentityErrors.UserNotFound(user.Id),
            "RoleNotFound" => Errors.IdentityErrors.RoleNotFound(string.Join(", ", roles)),
            "UserAlreadyInRole" => Errors.IdentityErrors.UserAlreadyInRole(user.Id, string.Join(", ", roles)),
            "ConcurrencyFailure" => Errors.IdentityErrors.ConcurrencyFailure(user.Id),
            _ => Errors.IdentityErrors.AddToRolesFailed(user.Id, roles, result.Errors)
          };
        }
      }

      await transaction.CommitAsync();
      return user;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      _logger.LogException(ex);
      return Errors.CommonErrors.Exception(ex.Message);
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
      return Errors.IdentityErrors.UserEmailNotFound(email);
    }

    var role = await _roleManager.FindByNameAsync(roleName);
    if (role == null)
    {
      return Errors.IdentityErrors.RoleNotFound(roleName);
    }

    var result = await _userManager.RemoveFromRoleAsync(user, roleName);
    if (!result.Succeeded)
    {
      _logger.LogIdentityErrors("_userManager.AddToRoleAsync", email, result.Errors.ToFormattedString());
      var firstError = result.Errors.FirstOrDefault();
      return firstError?.Code switch
      {
        "UserNotFound" => Errors.IdentityErrors.UserNotFound(user.Id),
        "RoleNotFound" => Errors.IdentityErrors.RoleNotFound(roleName),
        "UserNotInRole" => Errors.IdentityErrors.UserNotInRole(user.Id, roleName),
        "ConcurrencyFailure" => Errors.IdentityErrors.ConcurrencyFailure(user.Id),
        _ => Errors.IdentityErrors.RemoveFromRoleFailed(user.Id, roleName, result.Errors)
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
      return Errors.IdentityErrors.UserIdNotFound(userId);
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
      return Errors.IdentityErrors.UserNameNotFound(username);
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
        "DuplicateRoleName" => Errors.IdentityErrors.DuplicateRoleName(roleName),
        "InvalidRoleName" => Errors.IdentityErrors.InvalidRoleName(roleName),
        "ConcurrencyFailure" => Errors.IdentityErrors.ConcurrencyFailure(roleName),
        _ => Errors.IdentityErrors.CreateRoleFailed(roleName, result.Errors)
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
      return Errors.IdentityErrors.RoleNotFound(roleName);
    }

    var result = await _roleManager.DeleteAsync(role);
    if (!result.Succeeded)
    {
      _logger.LogIdentityErrors("_roleManager.DeleteAsync", "roleName", result.Errors.ToFormattedString());
      var firstError = result.Errors.FirstOrDefault();
      return firstError?.Code switch
      {
        "RoleNotFound" => Errors.IdentityErrors.RoleNotFound(roleName),
        "ConcurrencyFailure" => Errors.IdentityErrors.ConcurrencyFailure(roleName),
        _ => Errors.IdentityErrors.DeleteRoleFailed(roleName, result.Errors)
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
      return Errors.IdentityErrors.RoleNotFound(oldRoleName);
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
        "RoleNotFound" => Errors.IdentityErrors.RoleNotFound(role.Name),
        "DuplicateRoleName" => Errors.IdentityErrors.DuplicateRoleName(role.Name),
        "InvalidRoleName" => Errors.IdentityErrors.InvalidRoleName(role.Name),
        "ConcurrencyFailure" => Errors.IdentityErrors.ConcurrencyFailure(role.Name),
        _ => Errors.IdentityErrors.UpdateRoleFailed(oldRoleName, newRoleName, result.Errors)
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
      return Errors.IdentityErrors.RoleNotFound(roleName);
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

      return Errors.CommonErrors.Exception(ex.Message);
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
        (IList<string>)roles.ToList()
    };
    return userDto;
  }
}

