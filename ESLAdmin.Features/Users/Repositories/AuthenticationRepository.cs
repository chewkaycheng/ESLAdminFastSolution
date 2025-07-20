using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Users.Endpoints.GetUser;
using ESLAdmin.Features.Users.Endpoints.RegisterUser;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Features.Users.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Transactions;

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
  private readonly IConfiguration _configuration;
  private readonly UserDbContext _dbContext;

  //------------------------------------------------------------------------------
  //
  //                       AuthenticationRepository
  //
  //-------------------------------------------------------------------------------
  public AuthenticationRepository(
    IMessageLogger messageLogger,
    UserManager<User> userManager,
    UserDbContext dbContext,
    IConfiguration configuration)
  {
    _dbContext = dbContext;
    _messageLogger = messageLogger;
    _userManager = userManager;
    _configuration = configuration;
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
    try
    {
      var user = mapper.ToEntity(request);

      using var transaction = await _dbContext.Database.BeginTransactionAsync();

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
      _messageLogger.LogDatabaseException(
        nameof(RegisterUserAsync),
        ex);

      throw new DatabaseException(
        nameof(RegisterUserAsync), 
        ex);
    }
  }
  public async Task<UserResponse> GetUserByIdAsync(
    GetUserRequest request,
    GetUserMapper mapper)
  {
    try
    {
      var user = await _userManager.FindByIdAsync(request.Id);

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
        nameof(GetUserByIdAsync),
        ex);

      throw new DatabaseException(
        nameof(GetUserByIdAsync),
        ex);
    }
  }
}
