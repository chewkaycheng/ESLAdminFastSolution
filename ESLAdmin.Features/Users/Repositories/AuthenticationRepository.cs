using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Users.Endpoints.GetUser;
using ESLAdmin.Features.Users.Endpoints.RegisterUser;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Features.Users.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace ESLAdmin.Features.Users.Repositories;

//------------------------------------------------------------------------------
//
//                       class AuthenticationRepository
//
//-------------------------------------------------------------------------------
public class AuthenticationRepository : IAuthenticationRepository
{
  private readonly IMessageLogger _messageLogger;
  private readonly UserManager<User> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly UserDbContext _dbContext;

  //------------------------------------------------------------------------------
  //
  //                       AuthenticationRepository
  //
  //-------------------------------------------------------------------------------
  public AuthenticationRepository(
    IMessageLogger messageLogger,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    UserDbContext dbContext)
  {
    _dbContext = dbContext;
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
    RegisterUserRequest request, 
    RegisterUserMapper mapper)
  {
    IDbContextTransaction? transaction = null;
    try
    {
      var user = mapper.ToEntity(request);

      transaction = await _dbContext.Database.BeginTransactionAsync();

      var result = await _userManager.CreateAsync(
        user,
        request.Password);

      if (!result.Succeeded)
      {
        await transaction.RollbackAsync();
        return IdentityResultEx.Failed(result.Errors.ToArray());
      }

      if (request.Roles != null && request.Roles.Any())
      {
        var invalidRoles = new List<string>();
        foreach (var role in request.Roles)
        {
          if (!await _roleManager.RoleExistsAsync(role))
          {
            invalidRoles.Add(role);
          }
        }

        if (invalidRoles.Any())
        {
          await transaction.RollbackAsync();
          return IdentityResultEx.Failed(new IdentityError
          {
            Code = "InvalidRole",
            Description = $"The following roles do not exist: {string.Join(", ", invalidRoles)}"
          });
        }

        var roleResult = await _userManager.AddToRolesAsync(user, request.Roles);
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
  public async Task<UserResponse> GetUserByEmailAsync(
    GetUserRequest request,
    GetUserMapper mapper)
  {
    try
    {
      var user = await _userManager.FindByEmailAsync(request.Email);

      if (user == null)
      {
        return null;
      }

      var roles = await _userManager.GetRolesAsync(user);
      if (roles == null)
      {
        return mapper.FromEntity(user);
      }
      else
      {
        return mapper.FromEntityWithRoles(user, roles);
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
