using ESLAdmin.Features.ChildcareLevels.Mappers;
using ESLAdmin.Features.ChildcareLevels.Models;
using ESLAdmin.Features.Exceptions;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevel;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class GetChildcareLevelEndpoint : 
  Endpoint<
    EmptyRequest, 
    Results<Ok<ChildcareLevelResponse>, NotFound<APIErrors>, InternalServerError>,
    ChildcareLevelMapper>
{
  private readonly IRepositoryManager _manager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public GetChildcareLevelEndpoint(
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
    Get("/api/childcarelevels/{id}");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<ChildcareLevelResponse>, NotFound<APIErrors>, InternalServerError>> ExecuteAsync(
    EmptyRequest req, CancellationToken c)
  {
    try
    {
      var id = Route<int>("id");
      var childcareLevel = await _manager.ChildcareLevelRepository.GetChildcareLevel(
        id,
        Map);

      if (childcareLevel == null)
      {
        APIErrors errors = new APIErrors();
        ValidationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "NotFound",
          ErrorMessage = $"The childcare level with id: {id} is not found."
        });
        errors.Errors = ValidationFailures;
        return TypedResults.NotFound(errors);
      }
      return TypedResults.Ok(childcareLevel);    
    
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

      return TypedResults.InternalServerError();
    }
  }
}