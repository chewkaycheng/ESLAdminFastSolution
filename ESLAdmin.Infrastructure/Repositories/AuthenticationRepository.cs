using ESLAdmin.Common.Exceptions;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.Data;
using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Infrastructure.Repositories.Interfaces;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Text;
using static Google.Protobuf.Collections.MapField<TKey, TValue>;

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
    UserDbContext dbContext)
  {
    _dbContext = dbContext;
    _logger = logger;
    _messageLogger = messageLogger;
    _userManager = userManager;
    _signInManager = signInManager;
    _roleManager = roleManager;
  }

  //------------------------------------------------------------------------------
  //
  //                       RegisterUser
  //
  //-------------------------------------------------------------------------------
  public async Task<IdentityResultEx> RegisterUserAsync(
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
      {
        return IdentityResultEx.Failed(new IdentityError
        {
          Code = "InvalidRole",
          Description = $"The following roles do not exist: {string.Join(", ", invalidRoles)}"
        });
      }
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
        await transaction.RollbackAsync();
        return IdentityResultEx.Failed(result.Errors.ToArray());
      }

      if (roles != null && roles.Any())
      {
        var roleResult = await _userManager.AddToRolesAsync(user, roles);
        if (!roleResult.Succeeded)
        {
          await transaction.RollbackAsync();
          return IdentityResultEx.Failed(roleResult.Errors.ToArray());
        }
      }

      await transaction.CommitAsync();
      return IdentityResultEx.Success(user.Id);
    }
    catch (Exception ex)
    {
      if (transaction != null)
      {
        await transaction.RollbackAsync();
      }

      _messageLogger.LogDatabaseException(
        nameof(RegisterUserAsync),
        ex);

      throw new DatabaseException(
        nameof(RegisterUserAsync),
        ex);
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
  public async Task<(User user, ICollection<string>? roles)?> GetUserByEmailAsync(
    string email)
  {
    try
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user == null)
      {
        return null;
      }

      var roles = await _userManager.GetRolesAsync(user);
      if (roles == null)
      {
        return (user, null);
      }
      else
      {
        return (user, roles);
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(GetUserByEmailAsync),
        ex);

      throw new DatabaseException(
        nameof(GetUserByEmailAsync),
        ex);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       LoginAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<(User user, ICollection<string>? roles)?> LoginAsync(string email, string password)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
      return null;
    }

    var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
    if (!result.Succeeded)
    {
      return null;
    }

    var roles = await _userManager.GetRolesAsync(user);
    if (roles == null)
    {
      return (user, null);
    }
    else
    {
      return (user, roles);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       DeleteUserAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<IdentityResultEx> DeleteUserByEmailAsync(string email)
  {
    try
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return IdentityResultEx.Failed(new IdentityError
        {
          Code = "NotFound",
          Description = $"The user with email {email} is not found."
        });
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
      var deleteUserResult = await _userManager.DeleteAsync(user);
      if (!deleteUserResult.Succeeded)
      {
        InfoLogIdentityErrors("DeleteAsync", email, deleteUserResult.Errors);
        return IdentityResultEx.Failed(IdentityErrorTypes.DeleteUserError, deleteUserResult.Errors.ToArray());
      }

      return IdentityResultEx.Success(user.Id);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return IdentityResultEx.Failed(new IdentityError()
      {
        Code = "Exception",
        Description = ex.Message
      });
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       CreateRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<IdentityResultEx> CreateRoleAsync(string roleName)
  {
    try
    {
      if (await _roleManager.RoleExistsAsync(roleName))
      {
        return IdentityResultEx.Failed(new IdentityError
        {
          Code = "RoleExists",
          Description = $"Role '{roleName}' already exists."
        });
      }

      
      var role = new IdentityRole(roleName);
      var result = await _roleManager.CreateAsync(role);

      if (!result.Succeeded)
      {
        InfoLogIdentityErrors(nameof(CreateRoleAsync), roleName, result.Errors);
        return IdentityResultEx.Failed(IdentityErrorTypes.CreateRoleError, result.Errors.ToArray());
      }

      return IdentityResultEx.Success(roleName);  
      

    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return IdentityResultEx.Failed(new IdentityError()
      {
        Code = "Exception",
        Description = ex.Message
      });
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       UpdateRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<IdentityResultEx> UpdateRoleAsync(string oldRoleName, string newRoleName)
  {
    try
    {
      var role = await _roleManager.FindByNameAsync(oldRoleName);
      if (role == null)
      {
        return IdentityResultEx.Failed(new IdentityError
        {
          Code = "RoleNotFound",
          Description = $"Role '{oldRoleName}' not found."
        });
      }

      if (await _roleManager.RoleExistsAsync(newRoleName))
      {
        return IdentityResultEx.Failed(new IdentityError
        {
          Code = "RoleExists",
          Description = $"Role '{newRoleName}' already exists."
        });
      }

      role.Name = newRoleName;
      var result = await _roleManager.UpdateAsync(role);
      if (!result.Succeeded)
      {
        InfoLogIdentityErrors(nameof(UpdateRoleAsync), oldRoleName, result.Errors);
        return IdentityResultEx.Failed(IdentityErrorTypes.UpdateRoleError, result.Errors.ToArray());
      }

      return IdentityResultEx.Success(newRoleName);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return IdentityResultEx.Failed(new IdentityError()
      {
        Code = "Exception",
        Description = ex.Message
      });
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       DeleteRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<IdentityResultEx> DeleteRoleAsync(string roleName)
  {
    try
    {
      var role = await _roleManager.FindByNameAsync(roleName);
      if (role == null)
      {
        return IdentityResultEx.Failed(new IdentityError
        {
          Code = "RoleNotFound",
          Description = $"Role '{roleName}' not found."
        });
      }

      var result = await _roleManager.DeleteAsync(role);
      if (!result.Succeeded)
      {
        InfoLogIdentityErrors(nameof(UpdateRoleAsync), roleName, result.Errors);
        return IdentityResultEx.Failed(IdentityErrorTypes.DeleteRoleError, result.Errors.ToArray());
      }

      return IdentityResultEx.Success(roleName);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return IdentityResultEx.Failed(new IdentityError()
      {
        Code = "Exception",
        Description = ex.Message
      });
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetRoleAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<IdentityRole?> GetRoleAsync(string roleName)
  {
    try
    {
      return await _roleManager.FindByNameAsync(roleName);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      throw new DatabaseException(
        nameof(GetRoleAsync),
        ex);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetAllRolesAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
  {
    try
    {
      return await _roleManager.Roles.ToListAsync();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      throw new DatabaseException(
        nameof(GetAllRolesAsync),
        ex);
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
      foreach(var error in errors)
      {
        sb.AppendLine($"    Code: {error.Code}\tDescription: {error.Description}");
      }
      _logger.LogIdentityErrors(identityFunction, id, sb.ToString());
    }
  }
}
