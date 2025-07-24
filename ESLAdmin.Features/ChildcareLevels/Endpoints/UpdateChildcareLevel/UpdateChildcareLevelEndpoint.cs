using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.UpdateChildcareLevel;

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelEndpoint : Endpoint<
  UpdateChildcareLevelRequest,
  Results<Ok<UpdateChildcareLevelResponse>,
    ProblemDetails,
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
    ProblemDetails,
    InternalServerError>> ExecuteAsync(
      UpdateChildcareLevelRequest request, CancellationToken cancallationToken)
  {
    return await new UpdateChildcareLevelCommand
    {
      ChildcareLevelId = request.ChildcareLevelId,
      ChildcareLevelName = request.ChildcareLevelName,
      MaxCapacity = request.MaxCapacity,
      DisplayOrder = request.DisplayOrder,
      UserCode = request.UserCode,
      Guid = request.Guid,
      mapper = Map
    }.ExecuteAsync();

    //try
    //{
    //  OperationResult operationResult = await _manager.ChildcareLevelRepository.UpdateChildcareLevelAsync(
    //    request, Map);

    //  if (operationResult.DbApiError == 0)
    //  {
    //    UpdateChildcareLevelResponse response = new UpdateChildcareLevelResponse();
    //    response.ChildcareLevelId = request.ChildcareLevelId;
    //    response.Guid = operationResult.Guid;
    //    return TypedResults.Ok(response);
    //  }

    //  int statusCode;
    //  switch (operationResult.DbApiError)
    //  {
    //    case 100:
    //      {
    //        ValidationFailures.AddRange(new ValidationFailure
    //        {
    //          PropertyName = "ConcurrencyConflict",
    //          ErrorMessage = $"Another record with the childcare level name: {request.ChildcareLevelName} already exists."
    //        });
    //        statusCode = StatusCodes.Status409Conflict;
    //        break;
    //      }
    //    case 200:
    //      {
    //        ValidationFailures.AddRange(new ValidationFailure
    //        {
    //          PropertyName = "ConcurrencyConflict",
    //          ErrorMessage = $"The record has been altered by another user."
    //        });
    //        statusCode = StatusCodes.Status409Conflict;
    //        break;
    //      }
    //    case 300:
    //      {
    //        ValidationFailures.AddRange(new ValidationFailure
    //        {
    //          PropertyName = "NotFound",
    //          ErrorMessage = $"The record has does not exist."
    //        });
    //        statusCode = StatusCodes.Status404NotFound;
    //        break;
    //      }
    //    case 500:
    //    default:
    //      {
    //        ValidationFailures.AddRange(new ValidationFailure
    //        {
    //          PropertyName = "NotProcessed",
    //          ErrorMessage = $"The maximum capacity has been reached."
    //        });
    //        statusCode = StatusCodes.Status422UnprocessableEntity;
    //        break;
    //      }
    //  }

    //  return new ProblemDetails(ValidationFailures, statusCode);
    //}
    //catch (Exception ex)
    //{
    //  _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

    //  return TypedResults.InternalServerError();
    //}
  }
}