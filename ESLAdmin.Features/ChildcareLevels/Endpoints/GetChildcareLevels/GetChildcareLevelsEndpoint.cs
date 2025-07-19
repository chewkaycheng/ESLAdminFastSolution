using ESLAdmin.Features.ChildcareLevels.Mappers;
using ESLAdmin.Features.ChildcareLevels.Models;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevels;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelsEndpoint
//
//------------------------------------------------------------------------------
public class GetChildcareLevelsEndpoint : EndpointWithoutRequest<
  APIResponse<IEnumerable<ChildcareLevelResponse>>, 
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
  public override async Task HandleAsync(CancellationToken c)
  {
    try
    {
      var response = await _manager.ChildcareLevelRepository.GetChildcareLevels(Map);
      await SendAsync(response);
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

      var response = new APIResponse<IEnumerable<ChildcareLevelResponse>>();

      response.IsSuccess = false;
      response.Error = "Internal Server Error";
      await SendAsync(response, 500, c);
    }
  }
}