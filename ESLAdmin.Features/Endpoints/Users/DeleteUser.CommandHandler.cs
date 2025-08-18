using ErrorOr;
using ESLAdmin.Infrastructure.Persistence.Identity;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
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
  Results<Ok<Success>, ProblemDetails, InternalServerError>>
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
  public async Task<Results<Ok<Success>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      DeleteUserCommand command,
      CancellationToken cancellationToken)
  {
    _logger.LogFunctionEntry($"Email: {command.Email}");
    var result = await _repositoryManager.IdentityRepository.DeleteUserByEmailAsync(
      command.Email);

    if (result.IsError)
    {
      var error = result.Errors.First();
      return IdentityResultMapper.MapToResult<Success>(error);
    }

    _logger.LogFunctionExit($"Email: {command.Email}");
    return TypedResults.Ok(new Success());  
  }
}
