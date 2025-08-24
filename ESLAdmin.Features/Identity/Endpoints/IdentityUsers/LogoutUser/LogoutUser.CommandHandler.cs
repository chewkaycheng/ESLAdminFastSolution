using System.IdentityModel.Tokens.Jwt;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Endpoints.IdentityRoles;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Features.Identity.Services;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.LogoutUser;

//------------------------------------------------------------------------------
//
//                        class LogoutUserCommandHandler
//
//-------------------------------------------------------------------------------
public class LogoutUserCommandHandler : 
  IdentityCommandHandlerBase<LogoutUserCommandHandler>,
    ICommandHandler<LogoutUserCommand,
      Results<Ok, ProblemDetails, InternalServerError>>
{
  private readonly ITokenBlacklistService _tokenBlacklistService;

  public LogoutUserCommandHandler(
    IIdentityRepository repository,
    ILogger<LogoutUserCommandHandler> logger,
    ITokenBlacklistService tokenBlacklistService) :
    base(repository, logger)
  {
  }

  public async Task<Results<Ok, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      LogoutUserCommand command,
      CancellationToken ct)
  {
    var handler = new JwtSecurityTokenHandler();
    if (!handler.CanReadToken(command.Token))
    {
      _logger.LogCustomError($"Invalid JWT format for user: '{command.UserId}'.");
      return AppErrors.ProblemDetailsFactory.TokenError();
    }

    var jwtToken = handler.ReadJwtToken(command.Token);
    var expiryDate = jwtToken.ValidTo;

    try
    {
      // Revoke refresh tokens
      var result = await _repository
                        .RevokeRefreshTokenAsync(command.UserId, ct);
      if (result.IsError)
      {
        _logger.LogError($"Logout failed for user: {command.UserId}, error: {result.FirstError.Description}.");
        var error = result.Errors.FirstOrDefault();
        return error.Code switch
        {
          "Database.OperationCanceled" => AppErrors.ProblemDetailsFactory.RequestTimeout(),
          string code when code.Contains("Exception") => TypedResults.InternalServerError(),
          _ => AppErrors.ProblemDetailsFactory.InvalidLogoutRequest()
        };
      }

      // Blacklist JWT
      var blackListResult  = await _tokenBlacklistService.AddToBlacklistAsync(
        command.Token,
        command.UserId,
        expiryDate,
        ct);

      if (blackListResult.IsError)
      {
        _logger.LogError($"Logout failed for user: {command.UserId}, error: {result.FirstError.Description}.");
        var error = result.Errors.FirstOrDefault();
        return error.Code switch
        {
          "Database.OperationCanceled" => AppErrors.ProblemDetailsFactory.RequestTimeout(),
         _ => TypedResults.InternalServerError(),
        };
      }

      _logger.LogCustomInformation($"Successful logout for user: '{command.UserId}'."); 
      return TypedResults.Ok();
    }
    catch(Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }   
  }
}
