using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Users.Endpoints.GetUser;
using ESLAdmin.Features.Users.Endpoints.RegisterUser;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Features.Users.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
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
    RegisterUserCommand command)
    //RegisterUserRequest request, 
    //RegisterUserMapper mapper)
  {
    IDbContextTransaction? transaction = null;
    try
    {
      //var user = mapper.ToEntity(request);
      var user = command.Mapper.CommandToEntity(command); 
      transaction = await _dbContext.Database.BeginTransactionAsync();

      var result = await _userManager.CreateAsync(
        user,
        command.Password);
        //request.Password);

      if (!result.Succeeded)
      {
        await transaction.RollbackAsync();
        return IdentityResultEx.Failed(result.Errors.ToArray());
      }

      if (command.Roles != null && command.Roles.Any())
      //if (request.Roles != null && request.Roles.Any())
      {
        var invalidRoles = new List<string>();
        foreach (var role in command.Roles)
        //foreach (var role in request.Roles)
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

        var roleResult = await _userManager.AddToRolesAsync(user, command.Roles);
        //var roleResult = await _userManager.AddToRolesAsync(user, request.Roles);
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
    GetUserCommand command,
    GetUserMapper mapper)
  {
    try
    {
      var user = await _userManager.FindByEmailAsync(command.Email);

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
