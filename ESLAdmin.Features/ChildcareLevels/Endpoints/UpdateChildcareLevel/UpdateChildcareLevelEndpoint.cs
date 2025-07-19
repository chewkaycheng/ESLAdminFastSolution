using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using System.Runtime.CompilerServices;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.UpdateChildcareLevel;

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelEndpoint : Endpoint<
  UpdateChildcareLevelRequest, 
  APIResponse<OperationResult>, 
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
  //                           HandleAsync
  //
  //------------------------------------------------------------------------------
  public override async Task HandleAsync(UpdateChildcareLevelRequest r, CancellationToken c)
  {
    try
    {
      var response = await _manager.ChildcareLevelRepository.UpdateChildcareLevelAsync(r, Map);

      var httpResponseCode = ErrorUtils.MapHttpReturnCode(response.Data.DbApiError);
      await SendAsync(response, httpResponseCode, c);
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