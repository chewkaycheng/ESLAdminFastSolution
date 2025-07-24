using ESLAdmin.Common.Exceptions;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.Data;
using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Infrastructure.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
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
    RoleManager<IdentityRole> roleManager,
    UserDbContext dbContext)
  {
    _dbContext = dbContext;
    _logger = logger;
    _messageLogger = messageLogger;
    _userManager = userManager;
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
}
