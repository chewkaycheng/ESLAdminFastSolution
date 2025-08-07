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
//                        class DeleteUserCommandHandler
//
//-------------------------------------------------------------------------------
public class DeleteUserCommandHandler : ICommandHandler<
  DeleteUserCommand,
  Results<Ok<string>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetUserCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                        DeleteUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public DeleteUserCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<GetUserCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<string>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      DeleteUserCommand command,
      CancellationToken cancellationToken)
  {
    _logger.LogFunctionEntry($"Email: {command.Email}");
    try
    {
      var result = await _repositoryManager.AuthenticationRepository.DeleteUserByEmailAsync(
        command.Email);

      if (result.IsError)
      {
        var error = result.Errors.First();
        if (error.Code == "User.DeleteFailed")
        {
          return TypedResults.InternalServerError();
        }
        return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            error.Code, 
            error.Description), 
          StatusCodes.Status404NotFound);
      }

      _logger.LogFunctionExit($"Email: {command.Email}");
      return TypedResults.Ok(command.Email);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}
