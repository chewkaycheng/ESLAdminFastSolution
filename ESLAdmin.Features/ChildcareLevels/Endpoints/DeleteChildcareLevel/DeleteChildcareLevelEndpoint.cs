using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.DeleteChildcareLevel;

//------------------------------------------------------------------------------
//
//                           class DeleteChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class DeleteChildcareLevelEndpoint : Endpoint<
  DeleteChildcareLevelRequest, 
  Results<NoContent, Conflict<APIErrors>, InternalServerError>,
  DeleteChildcareLevelMapper>
{
  private readonly IRepositoryManager _manager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                           DeleteChildcareLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public DeleteChildcareLevelEndpoint(
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
    Delete("/api/childcarelevels/{id}");
  }

  //------------------------------------------------------------------------------
  //
  //                           ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<NoContent, Conflict<APIErrors>, InternalServerError>> ExecuteAsync(
    DeleteChildcareLevelRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var id = Route<int>("id");
      var operationResult = await _manager.ChildcareLevelRepository.DeleteChildcareLevel(
        id, 
        Map);
      if (operationResult.DbApiError == 0)
      {
        return TypedResults.NoContent();
      }
      else
      {
        APIErrors errors = new APIErrors();
        ValidationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "ConcurrencyConflict",
          ErrorMessage = $"Cannot delete Childcare level. It is being used by {operationResult.ReferenceTable}."
        });
        return TypedResults.Conflict(errors);
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

      return TypedResults.InternalServerError();
    }
  }
}