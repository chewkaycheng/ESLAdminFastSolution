using ESLAdmin.Domain.Entities;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ESLAdmin.Features.Users.Repositories.Interfaces;
using ESLAdmin.Features.Users.RegisterUser;
using Microsoft.AspNetCore.Mvc;
using ESLAdmin.Features.Exceptions;

namespace ESLAdmin.Features.Users.Repositories;

public class AuthenticationRepository : IAuthenticationRepository
{
  private readonly IMessageLogger _messageLogger;
  private readonly UserManager<User> _userManager;
  private readonly IConfiguration _configuration;

  public AuthenticationRepository(
    IMessageLogger messageLogger,
    UserManager<User> userManager,
    IConfiguration configuration)
  {
    _messageLogger = messageLogger;
    _userManager = userManager;
    _configuration = configuration;
  }

  public async Task<APIResponse<IdentityResultExtended>> RegisterUser(
    Request request, 
    Mapper mapper)
  {
    try
    {
      var user = mapper.ToEntity(request);

      var result = await _userManager.CreateAsync(
        user, 
        request.Password);

      if (result.Succeeded)
        await _userManager.AddToRolesAsync(
          user, 
          request.Roles);

      IdentityResultExtended resultExtended = new IdentityResultExtended();
      resultExtended.IdentityResult = result;
      resultExtended.User = user;

      var response = new APIResponse<IdentityResultExtended>();
      response.IsSuccess = result.Succeeded;
      response.Data = resultExtended;
      return response;
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(RegisterUser),
        ex);

      throw new DatabaseException(
        nameof(RegisterUser), 
        ex);
    }
  }
}
