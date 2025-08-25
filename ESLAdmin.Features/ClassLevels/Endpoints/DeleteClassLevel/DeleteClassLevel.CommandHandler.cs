using Dapper;
using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ClassLevels.Endpoints.DeleteClassLevel;

//------------------------------------------------------------------------------
//
//                    class DeleteClassLevelCommandHandler
//
//------------------------------------------------------------------------------
public class DeleteClassLevelCommandHandler :
  ClassLevelCommandHandlerBase<DeleteClassLevelCommandHandler>,
  ICommandHandler<DeleteClassLevelCommand,
    Results<Ok<Success>,
      ProblemDetails,
      InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                    DeleteClassLevelCommandHandler
  //
  //------------------------------------------------------------------------------
  public DeleteClassLevelCommandHandler(
  IClassLevelRepository repository,
    ILogger<DeleteClassLevelCommandHandler> logger) : 
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                    ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<Success>, 
    ProblemDetails, 
    InternalServerError>> 
      ExecuteAsync(
        DeleteClassLevelCommand command, 
        CancellationToken ct)
  {
    _logger.LogFunctionEntry($"\n=>ClassLevelId: '{command.Id}'");
    DynamicParameters parameters = command.Mapper.ToParameters(command.Id);
    var result = await _repository.
      DeleteClassLevelAsync(
        parameters);

    if (result.IsError)
    {
      var errors = result.Errors;
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(errors),
        StatusCodes.Status500InternalServerError);
    }

    OperationResult operationResult = command.Mapper.FromParameters(parameters);
    if (operationResult.DbApiError == 0)
    {
      _logger.LogFunctionExit();
      return TypedResults.Ok(new Success());
    }

    _logger.LogFunctionExit();
    return new ProblemDetails(ErrorUtils.CreateFailureList(
        "Database.ConcurrencyConflict",
        $"Cannot delete Class level. It is being used by {operationResult.ReferenceTable}."),
      StatusCodes.Status409Conflict);
  }
}
