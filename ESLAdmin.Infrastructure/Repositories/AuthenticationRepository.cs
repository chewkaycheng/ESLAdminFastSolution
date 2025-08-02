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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace ESLAdmin.Features.Users.Repositories;

//------------------------------------------------------------------------------
//
//                       class AuthenticationRepository
//
//-------------------------------------------------------------------------------
public class AuthenticationRepository : IAuthenticationRepository
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
  //                       AuthenticationRepository
  //
  //-------------------------------------------------------------------------------
  public AuthenticationRepository(
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
  //                       GetAllUsersAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<IEnumerable<UserDto>>> GetAllUsersAsync()
  {
    try
    {
      var sql = DbConstsIdentity.SQL_GETALL;

      using IDbConnection connection = await _dbContextDapper.GetConnectionAsync();
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
            (IList<string>) userRoles.Where(r => r.UserId == user.Id)
            .Select(r => r.Name)
            .ToList()
        };
        userDtos.Add(userDto);
      }

      return userDtos;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
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
    // Check for any invalid roles before doing transaction
    if (roles != null && roles.Any())
    {
      var invalidRoles = new List<string>();
      foreach (var role in roles)
      {
        if (!await _roleManager.RoleExistsAsync(role))
        {
          invalidRoles.Add(role);
        }
      }

      if (invalidRoles.Any())
        return Errors.IdentityErrors.InvalidRoles(invalidRoles);
    }

    IDbContextTransaction? transaction = null;
    try
    {
      transaction = await _dbContext.Database.BeginTransactionAsync();

      var result = await _userManager.CreateAsync(
        user,
        password);

      if (!result.Succeeded)
      {
        _logger.LogIdentityErrors(
          "_userManager.CreateAsync",
          user.Email,
          result.Errors.ToFormattedString());

        await transaction.RollbackAsync();
        return Errors.IdentityErrors.CreateUserFailed(user.Email, result.Errors);
      }

      if (roles != null && roles.Any())
      {
        var roleResult = await _userManager.AddToRolesAsync(user, roles);
        if (!roleResult.Succeeded)
        {
          _logger.LogIdentityErrors(
            "_userManager.AddToRolesAsync",
            user.Email,
            result.Errors.ToFormattedString());

          await transaction.RollbackAsync();
          return Errors.IdentityErrors.AddToRolesFailed(user.Email, result.Errors);
        }
      }

      await transaction.CommitAsync();
      return user;
    }
    catch (Exception ex)
    {
      if (transaction != null)
      {
        await transaction.RollbackAsync();
      }

      _logger.LogException(ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
    finally
    {
      transaction?.Dispose();
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
    try
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user == null)
      {
        return Errors.IdentityErrors.UserNotFound(email);
      }

      var roles = await _userManager.GetRolesAsync(user);
      return MapToDto(user, roles);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       LoginAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<UserDto>> LoginAsync(string email, string password)
  {
    try
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return Errors.IdentityErrors.UserLoginFailed();
      }

      var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
      if (!result.Succeeded)
      {
        return Errors.IdentityErrors.UserLoginFailed();
      }

      var roles = await _userManager.GetRolesAsync(user);
      return MapToDto(user, roles);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       DeleteUserAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<string>> DeleteUserByEmailAsync(string email)
  {
    try
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return Errors.IdentityErrors.UserNotFound(email);
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
       
        return Errors.IdentityErrors.DeleteUserFailed(email, result.Errors);
      }

      return email;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       CreateRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<IdentityRole>> CreateRoleAsync(string roleName)
  {
    try
    {
      if (await _roleManager.RoleExistsAsync(roleName))
      {
        return Errors.IdentityErrors.RoleExists(roleName);
      }


      var role = new IdentityRole(roleName);
      var result = await _roleManager.CreateAsync(role);

      if (!result.Succeeded)
      {
        _logger.LogIdentityErrors("_roleManager.CreateAsync", roleName, result.Errors.ToFormattedString());
        Errors.IdentityErrors.CreateRoleFailed(roleName, result.Errors);
      }

      return role;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       UpdateRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<string>> UpdateRoleAsync(string oldRoleName, string newRoleName)
  {
    try
    {
      var role = await _roleManager.FindByNameAsync(oldRoleName);
      if (role == null)
      {
        return Errors.IdentityErrors.RoleNotFound(oldRoleName);
      }

      if (await _roleManager.RoleExistsAsync(newRoleName))
      {
        return Errors.IdentityErrors.RoleExists(newRoleName);
      }

      role.Name = newRoleName;
      var result = await _roleManager.UpdateAsync(role);
      if (!result.Succeeded)
      {
        _logger.LogIdentityErrors("_roleManager.UpdateAsync", newRoleName, result.Errors.ToFormattedString());
        return Errors.IdentityErrors.UpdateRoleFailed(oldRoleName, newRoleName, result.Errors);
      }

      return newRoleName;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       DeleteRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<string>> DeleteRoleAsync(string roleName)
  {
    try
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
        return Errors.IdentityErrors.DeleteRoleFailed(roleName, result.Errors);
      }

      return role.Id;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<ErrorOr<IdentityRole>> GetRoleAsync(string roleName)
  {
    try
    {
      var role = await _roleManager.FindByNameAsync(roleName);
      if (role == null)
      {
        return Errors.IdentityErrors.RoleNotFound(roleName);
      }
      return role;
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
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
      Roles = roles
    };
    return userDto;
  }
}

