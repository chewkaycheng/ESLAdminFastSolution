using Dapper;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels.UpdateChildcareLevel;

public class UpdateChildcareLevelCommandHandler : 
  ChildcareLevelCommandHandlerBase<UpdateChildcareLevelCommandHandler>,
  ICommandHandler<UpdateChildcareLevelCommand,
    Results<Ok<UpdateChildcareLevelResponse>, ProblemDetails, InternalServerError>>
{
  public UpdateChildcareLevelCommandHandler(
    IChildcareLevelRepository repository,
    ILogger<UpdateChildcareLevelCommandHandler> logger) :
    base(repository, logger)
  {
  }

  public async Task<Results<Ok<UpdateChildcareLevelResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      UpdateChildcareLevelCommand command,
      CancellationToken cancellationToken)
  {
    DynamicParameters parameters = command.Mapper.ToParameters(command);
    var result = await _repository
      .UpdateChildcareLevelAsync(
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
      if (operationResult.Guid == null)
      {
        _logger.LogError("Guid Id for UpdateChildcareLevel is null");
        return TypedResults.InternalServerError();
      }

      UpdateChildcareLevelResponse response = new UpdateChildcareLevelResponse
      {
        ChildcareLevelId = command.ChildcareLevelId,
        Guid = operationResult.Guid
      };
      return TypedResults.Ok(response);
    }

    int statusCode;
    var validationFailures = new List<ValidationFailure>();
    switch (operationResult.DbApiError)
    {
      case 100:
      {
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "ConcurrencyConflict",
          ErrorMessage = $"Another record with the childcare level name: {command.ChildcareLevelName} already exists."
        });
        statusCode = StatusCodes.Status409Conflict;
        break;
      }
      case 200:
      {
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "ConcurrencyConflict",
          ErrorMessage = $"The record has been altered by another user."
        });
        statusCode = StatusCodes.Status409Conflict;
        break;
      }
      case 300:
      {
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "NotFound",
          ErrorMessage = $"The record has does not exist."
        });
        statusCode = StatusCodes.Status404NotFound;
        break;
      }
      case 500:
      default:
      {
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "NotProcessed",
          ErrorMessage = $"The maximum capacity has been reached."
        });
        statusCode = StatusCodes.Status422UnprocessableEntity;
        break;
      }
    }

    return new ProblemDetails(validationFailures, statusCode);
  }
}