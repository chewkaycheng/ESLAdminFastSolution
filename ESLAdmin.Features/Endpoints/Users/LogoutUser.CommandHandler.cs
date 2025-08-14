using ErrorOr;
using ESLAdmin.Common.Errors;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

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


  //------------------------------------------------------------------------------
  //
  //                        LogoutUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public LogoutUserCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<LogoutUserCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok, ProblemDetails, InternalServerError>> 
    ExecuteAsync(
      LogoutUserCommand command, 
      CancellationToken ct)
  {
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

    _logger.LogCustomInformation($"Successful logout for user: '{command.UserId}'.");
    return TypedResults.Ok();
  }
}
