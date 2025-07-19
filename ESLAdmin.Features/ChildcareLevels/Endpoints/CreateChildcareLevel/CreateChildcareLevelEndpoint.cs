using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.CreateChildcareLevel;

//------------------------------------------------------------------------------
//
//                        Class CreateChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelEndpoint : Endpoint<
  CreateChildcareLevelRequest, 
  APIResponse<OperationResult>, 
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
  //                        HandleAsync
  //
  //------------------------------------------------------------------------------
  public override async Task HandleAsync(CreateChildcareLevelRequest r, CancellationToken c)
  {
    try
    {
      var response = await _manager.ChildcareLevelRepository.CreateChildcareLevelAsync(r, Map);
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