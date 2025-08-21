using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Infrastructure.Services;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        class LogoutUserCommandHandler
//
//-------------------------------------------------------------------------------
public class LogoutUserCommandHandler : ICommandHandler<
  LogoutUserCommand,
  Results<Ok, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<LogoutUserCommandHandler> _logger;
  private readonly ITokenBlacklistService _tokenBlacklistService;

  public LogoutUserCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<LogoutUserCommandHandler> logger,
    ITokenBlacklistService tokenBlacklistService)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _tokenBlacklistService = tokenBlacklistService;
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
      var result = await _repositoryManager
                        .IdentityRepository
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
