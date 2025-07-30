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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

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
      if (string.IsNullOrEmpty(email))
      {
        return IdentityResultEx.Failed(new IdentityError
        {
          Code = "NullOrEmpty",
          Description = "Email cannot be null or emtpy"
        });
      }

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
      var roles = await _userManager.GetRolesAsync(user);
      if (roles.Any())
      {
        var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!removeRolesResult.Succeeded) 
        {
          return IdentityResultEx.Failed(removeRolesResult.Errors.ToArray());
        }
      }

      // Delete the user
      var result = await _userManager.DeleteAsync(user);
      if (!result.Succeeded)
      {
        return IdentityResultEx.Failed(result.Errors.ToArray());
      }

      return IdentityResultEx.Success(user.Id);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      IdentityResultEx result = IdentityResultEx.Failed(new IdentityError()
      {
        Code = "Exception",
        Description = ex.Message
      });
    }
  }
}
