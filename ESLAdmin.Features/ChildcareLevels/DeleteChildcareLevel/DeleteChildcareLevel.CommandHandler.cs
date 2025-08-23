using Dapper;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels.DeleteChildcareLevel;

//------------------------------------------------------------------------------
//
//                    class DeleteChildcareLevelCommandHandler
//
//------------------------------------------------------------------------------
public class DeleteChildcareLevelCommandHandler :
  ICommandHandler<DeleteChildcareLevelCommand,
    Results<NoContent, ProblemDetails, InternalServerError>>
{
  private readonly IChildcareLevelRepository _repository;
  private readonly ILogger<DeleteChildcareLevelCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                    DeleteChildcareLevelCommandHandler
  //
  //------------------------------------------------------------------------------
  public DeleteChildcareLevelCommandHandler(
    IChildcareLevelRepository repository,
    ILogger<DeleteChildcareLevelCommandHandler> logger,
    IMessageLogger messageLogger)
  {
    _repository = repository;
    _logger = logger;
    _messageLogger = messageLogger;
  }

  //------------------------------------------------------------------------------
  //
  //                    DeleteChildcareLevelCommand
  //
  //------------------------------------------------------------------------------
  public async Task<Results<NoContent, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      DeleteChildcareLevelCommand command,
      CancellationToken cancellationToken)
  {
    _logger.LogFunctionEntry($"\n=>ChildcareLevelId: {command.Id}");
    DynamicParameters parameters = command.Mapper.ToParameters(command.Id);
    var result = await _repository
      .DeleteChildcareLevelAsync(
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
      return TypedResults.NoContent();
    }

    _logger.LogFunctionExit();
    return new ProblemDetails(ErrorUtils.CreateFailureList(
        "ConcurrencyConflict",
        $"Cannot delete Childcare level. It is being used by {operationResult.ReferenceTable}."),
      StatusCodes.Status409Conflict);
  }
}