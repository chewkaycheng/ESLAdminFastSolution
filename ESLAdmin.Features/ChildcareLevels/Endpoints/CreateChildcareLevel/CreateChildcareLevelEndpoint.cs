using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.CreateChildcareLevel;

//------------------------------------------------------------------------------
//
//                        Class CreateChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelEndpoint : Endpoint<
  CreateChildcareLevelRequest, 
  Results<NoContent, Conflict<APIErrors>, InternalServerError>, 
  CreateChildcareLevelMapper>
{
  private readonly IRepositoryManager _manager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                        CreateChildcareLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public CreateChildcareLevelEndpoint(
    IRepositoryManager manager,
    IMessageLogger messageLogger)
  {
    _manager = manager;
    _messageLogger = messageLogger;
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Post("/api/childcarelevels");
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<NoContent, Conflict<APIErrors>, InternalServerError> > ExecuteAsync(CreateChildcareLevelRequest request, CancellationToken canellationToken)
  {
    try
    {
      var operationResult = await _manager.ChildcareLevelRepository.CreateChildcareLevelAsync(request, Map);
      if (operationResult.DbApiError == 0)
      {
        HttpContext.Response.Headers.Append(
        "location", $"/api/childcarelevels/{operationResult.Id}");
        return TypedResults.NoContent();
      }
      else
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
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

      var response = new APIResponse<OperationResult>();

      return TypedResults.InternalServerError();
    }
  }
}