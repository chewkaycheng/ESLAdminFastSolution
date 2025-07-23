using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Features.Users.Endpoints.GetUser;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.UpdateChildcareLevel;

public class UpdateChildcareLevelCommandHandler : ICommandHandler<
  UpdateChildcareLevelCommand,
  Results<Ok<UpdateChildcareLevelResponse>,
  ProblemDetails,
  InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetUserCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  public UpdateChildcareLevelCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<GetUserCommandHandler> logger,
      IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _messageLogger = messageLogger;
  }

  public async Task<Results<Ok<UpdateChildcareLevelResponse>, ProblemDetails, InternalServerError>>
   ExecuteAsync(
     UpdateChildcareLevelCommand command,
     CancellationToken cancellationToken)
  {
    try
    {
      OperationResult operationResult = await _repositoryManager.ChildcareLevelRepository.UpdateChildcareLevelAsync(
        command);

      if (operationResult.DbApiError == 0)
      {
        UpdateChildcareLevelResponse response = new UpdateChildcareLevelResponse();
        response.ChildcareLevelId = command.ChildcareLevelId;
        response.Guid = operationResult.Guid;
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
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(ExecuteAsync), ex);

      return TypedResults.InternalServerError();
    }
  }
}
