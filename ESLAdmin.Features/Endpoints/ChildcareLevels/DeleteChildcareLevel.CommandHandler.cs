using Dapper;
using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
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
  private readonly ILogger<CreateChildcareLevelCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                    DeleteChildcareLevelCommandHandler
  //
  //------------------------------------------------------------------------------
  public DeleteChildcareLevelCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<CreateChildcareLevelCommandHandler> logger,
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
    try
    {
      DynamicParameters parameters = command.Mapper.ToEntity(command.Id);
      await _repositoryManager
              .ChildcareLevelRepository
              .DeleteChildcareLevelAsync(
                parameters);

      OperationResult operationResult = command.Mapper.FromEntity(parameters);
      if (operationResult.DbApiError == 0)
      {
        return TypedResults.NoContent();
      }
      else
      {
        var validationFailures = new List<ValidationFailure>();
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "ConcurrencyConflict",
          ErrorMessage = $"Cannot delete Childcare level. It is being used by {operationResult.ReferenceTable}."
        });
        return new ProblemDetails(validationFailures, StatusCodes.Status409Conflict);
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogControllerException(
        nameof(ExecuteAsync),
        ex);

      return TypedResults.InternalServerError();
    }
  }
}
