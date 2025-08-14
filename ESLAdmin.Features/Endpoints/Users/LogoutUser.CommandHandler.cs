using ErrorOr;
using ESLAdmin.Common.Errors;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Infrastructure.Services;
using ESLAdmin.Infrastructure.Utilities;
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
    // Validate JWT token
    if (string.IsNullOrEmpty(command.Token))
    {
      _logger.LogCustomError(
        $"No JWT provided for user: '{command.UserId}'");
      return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            "Authentication Error",
            "No JWT provided."),
            StatusCodes.Status401Unauthorized);
    }

    var handler = new JwtSecurityTokenHandler();
    if (!handler.CanReadToken(command.Token))
    {
      _logger.LogCustomError($"Invalid JWT format for user: '{command.UserId}'.");
      return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            "Authentication Error",
            "Invalid JWT format."),
            StatusCodes.Status401Unauthorized);
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
        if (result.FirstError.Type == ErrorType.NotFound)
        {
          return new ProblemDetails(
            ErrorUtils.CreateFailureList(
              result.FirstError.Code,
              result.FirstError.Description),
              StatusCodes.Status404NotFound);
        }
        return TypedResults.InternalServerError();
      }

      // Blacklist JWT
      await _tokenBlacklistService.AddToBlacklistAsync(
        command.Token,
        command.UserId,
        expiryDate,
        ct);

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
