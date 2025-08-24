using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Endpoints.IdentityRoles;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.DeleteUser;

//------------------------------------------------------------------------------
//
//                        class DeleteUserCommandHandler
//
//-------------------------------------------------------------------------------
public class DeleteUserCommandHandler : 
  IdentityCommandHandlerBase<DeleteUserCommandHandler>,
    ICommandHandler<
      DeleteUserCommand,
        Results<
          Ok<Success>, 
          ProblemDetails, 
          InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                        DeleteUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public DeleteUserCommandHandler(
      IIdentityRepository repository,
      ILogger<DeleteUserCommandHandler> logger) :
    base(repository, logger)
  {
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
    var result = await _repository
      .DeleteUserByEmailAsync(command.Email);

    if (result.IsError)
    {
      var error = result.Errors.First();
      var statusCode = error.Code switch
      {
        "Database.ConcurrencyFailure" => StatusCodes.Status409Conflict,
        "Database.OperationCanceled" => StatusCodes.Status408RequestTimeout,
        string code when code.Contains("Exception") => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest
      };

      return AppErrors
        .ProblemDetailsFactory
        .CreateProblemDetails(
          error.Code,
          error.Description,
          statusCode);
    }

    _logger.LogFunctionExit($"Email: {command.Email}");
    return TypedResults.Ok(new Success());  
  }
}
