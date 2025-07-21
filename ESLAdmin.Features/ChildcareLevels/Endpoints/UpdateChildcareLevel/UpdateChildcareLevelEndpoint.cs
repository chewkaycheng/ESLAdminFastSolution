using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.CompilerServices;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.UpdateChildcareLevel;

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelEndpoint : Endpoint<
  UpdateChildcareLevelRequest,
  Results<Ok<UpdateChildcareLevelResponse>,
    Conflict<APIErrors>,
    NotFound<APIErrors>,
    UnprocessableEntity<APIErrors>,
    InternalServerError>,
  UpdateChildcareLevelMapper>
{
  private readonly IRepositoryManager _manager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                           UpdateChildcareLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public UpdateChildcareLevelEndpoint(
    IRepositoryManager manager,
    IMessageLogger messageLogger)
  {
    _manager = manager;
    _messageLogger = messageLogger;
  }

  //------------------------------------------------------------------------------
  //
  //                           Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Put("/api/childcarelevels");
  }

  //------------------------------------------------------------------------------
  //
  //                           ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<UpdateChildcareLevelResponse>,
    Conflict<APIErrors>,
    NotFound<APIErrors>,
    UnprocessableEntity<APIErrors>,
    InternalServerError>> ExecuteAsync(
      UpdateChildcareLevelRequest request, CancellationToken cancallationToken)
  {
    try
    {
      OperationResult operationResult = await _manager.ChildcareLevelRepository.UpdateChildcareLevelAsync(
        request, Map);
        
      switch (operationResult.DbApiError)
      {
        case 100:
          {
            APIErrors errors = new APIErrors();
            ValidationFailures.AddRange(new ValidationFailure
            {
              PropertyName = "ConcurrencyConflict",
              ErrorMessage = $"Another record with the childcare level name: {request.ChildcareLevelName} already exists."
            });
            errors.Errors = ValidationFailures;
            return TypedResults.Conflict(errors);
          }
        case 200:
          {
            APIErrors errors = new APIErrors();
            ValidationFailures.AddRange(new ValidationFailure
            {
              PropertyName = "ConcurrencyConflict",
              ErrorMessage = $"The record has been altered by another user."
            });
            errors.Errors = ValidationFailures;
            return TypedResults.Conflict(errors);
          }
        case 300:
          {
            APIErrors errors = new APIErrors();
            ValidationFailures.AddRange(new ValidationFailure
            {
              PropertyName = "NotFound",
              ErrorMessage = $"The record has does not exist."
            });
            errors.Errors = ValidationFailures;
            return TypedResults.NotFound(errors);
          }
        case 500:
          {
            APIErrors errors = new APIErrors();
            ValidationFailures.AddRange(new ValidationFailure
            {
              PropertyName = "NotProcessed",
              ErrorMessage = $"The maximum capacity has been reached."
            });
            errors.Errors = ValidationFailures;
            return TypedResults.UnprocessableEntity(errors);
          }
        default:
          UpdateChildcareLevelResponse response = new UpdateChildcareLevelResponse();
          response.ChildcareLevelId = request.ChildcareLevelId;
          response.Guid = operationResult.Guid;
          return TypedResults.Ok(response);
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

      return TypedResults.InternalServerError();
    }
  }
}