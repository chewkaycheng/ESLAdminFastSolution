using Dapper;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.DeleteChildcareLevel;

public class Endpoint : Endpoint<Request, APIResponse<OperationResult>, Mapper>
{
  private readonly IRepositoryManager _manager;
  private readonly IMessageLogger _messageLogger;
  public Endpoint(
    IRepositoryManager manager,
    IMessageLogger messageLogger)
  {
    _manager = manager;
    _messageLogger = messageLogger;
  }

  public override void Configure()
  {
    Delete("/api/childcarelevels/{id}");
  }

  public override async Task HandleAsync(Request r, CancellationToken c)
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