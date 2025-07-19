using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.DeleteChildcareLevel;

//------------------------------------------------------------------------------
//
//                           class DeleteChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class DeleteChildcareLevelEndpoint : Endpoint<
  DeleteChildcareLevelRequest, 
  APIResponse<OperationResult>, 
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
  //                           HandleAsync
  //
  //------------------------------------------------------------------------------
  public override async Task HandleAsync(DeleteChildcareLevelRequest r, CancellationToken c)
  {
    try
    {
      var id = Route<int>("id");
      var response = await _manager.ChildcareLevelRepository.DeleteChildcareLevel(id, Map);
      await SendAsync(
        response, response.Data.DbApiError == 0 ? 200 : 409, c);
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

      var response = new APIResponse<OperationResult>();

      response.IsSuccess = false;
      response.Error = "Internal Server Error";
      await SendAsync(response, 500, c);
    }
  }
}