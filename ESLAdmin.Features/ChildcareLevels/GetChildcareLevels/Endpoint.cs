using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.GetChildcareLevels;

public class Endpoint : EndpointWithoutRequest<APIResponse<IEnumerable<Response>>, Mapper>
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
    Get("/api/childcarelevels");
    AllowAnonymous();
  }

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

      var response = new APIResponse<IEnumerable<Response>>();

      response.IsSuccess = false;
      response.Error = "Internal Server Error";
      await SendAsync(response, 500, c);
    }
  }
}