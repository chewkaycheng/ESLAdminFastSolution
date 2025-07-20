using ESLAdmin.Features.ChildcareLevels.Mappers;
using ESLAdmin.Features.ChildcareLevels.Models;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevels;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelsEndpoint
//
//------------------------------------------------------------------------------
public class GetChildcareLevelsEndpoint : EndpointWithoutRequest<
  Results<Ok<IEnumerable<ChildcareLevelResponse>>, InternalServerError>, 
  ChildcareLevelMapper>
{
  private readonly IRepositoryManager _manager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevelsEndpoint
  //
  //------------------------------------------------------------------------------
  public GetChildcareLevelsEndpoint(
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
    Get("/api/childcarelevels");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        HandleAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<IEnumerable<ChildcareLevelResponse>>, InternalServerError>> ExecuteAsync(CancellationToken c)
  {
    try
    {
      var childcarelevels = await _manager.ChildcareLevelRepository.GetChildcareLevels(Map);
      return TypedResults.Ok(childcarelevels);
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

      return TypedResults.InternalServerError();
    }
  }
}