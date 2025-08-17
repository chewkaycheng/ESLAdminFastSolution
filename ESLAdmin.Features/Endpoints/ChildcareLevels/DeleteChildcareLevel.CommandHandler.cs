using Dapper;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                    class DeleteChildcareLevelCommandHandler
//
//------------------------------------------------------------------------------
public class DeleteChildcareLevelCommandHandler :
  ICommandHandler<DeleteChildcareLevelCommand,
    Results<NoContent, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<DeleteChildcareLevelCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                    DeleteChildcareLevelCommandHandler
  //
  //------------------------------------------------------------------------------
  public DeleteChildcareLevelCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<DeleteChildcareLevelCommandHandler> logger,
      IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
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
    try
    {
      DynamicParameters parameters = command.Mapper.ToParameters(command.Id);
      var result = await _repositoryManager
              .ChildcareLevelRepository
              .DeleteChildcareLevelAsync(
                parameters);

      if (result.IsError)
      {
        var error = result.Errors.First();
        if (error.Code != "Database.StoredProcedureError")
        {
          return TypedResults.InternalServerError();
        }
        OperationResult operationResult = command.Mapper.FromParameters(parameters);
        if (operationResult.DbApiError == 0)
        {
          _logger.LogFunctionExit();
          return TypedResults.NoContent();
        }
        else
        {
          _logger.LogFunctionExit();
          return new ProblemDetails(ErrorUtils.CreateFailureList(
            "ConcurrencyConflict",
            $"Cannot delete Childcare level. It is being used by {operationResult.ReferenceTable}."),
            StatusCodes.Status409Conflict);
        }
      }
      return TypedResults.NoContent();
    }
    catch (Exception exception)
    {
      _logger.LogException(exception);
      return TypedResults.InternalServerError();
    }
  }
}
