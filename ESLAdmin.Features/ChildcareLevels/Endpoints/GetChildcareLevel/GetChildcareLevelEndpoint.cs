using ESLAdmin.Features.ChildcareLevels.Mappers;
using ESLAdmin.Features.ChildcareLevels.Models;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevel;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class GetChildcareLevelEndpoint : 
  Endpoint<
    EmptyRequest, 
    APIResponse<ChildcareLevelResponse>, 
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
  //                        HandleAsync
  //
  //------------------------------------------------------------------------------
  public override async Task HandleAsync(EmptyRequest req, CancellationToken c)
  {
    try
    {
      var id = Route<int>("id");
      var response = await _manager.ChildcareLevelRepository.GetChildcareLevel(
        id,
        Map);
      await SendAsync(
       response, response.IsSuccess ? 200 : 404, c);

    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

      var apiResponse = new APIResponse<ChildcareLevelResponse>();

      apiResponse.IsSuccess = false;
      apiResponse.Error = "Internal Server Error";
      await SendAsync(apiResponse, 500, c);
    }
  }
}