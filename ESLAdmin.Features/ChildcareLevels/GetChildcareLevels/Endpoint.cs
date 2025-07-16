using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.GetChildcareLevels;

public class Endpoint : EndpointWithoutRequest<APIResponse<IEnumerable<Response>>, Mapper>
{
  private readonly IRepositoryBase<ChildcareLevel, OperationResult> _repository;
  private readonly IMessageLogger _messageLogger;
  public Endpoint(
    IRepositoryBase<ChildcareLevel, OperationResult> repository,
    IMessageLogger messageLogger)
  {
    _repository = repository;
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
      var sql = DbConstsChildcareLevel.SQL_GETALL;

      var childcareLevels = await _repository.DapQueryMultipleAsync(sql, null);
      IEnumerable<Response> childcareLevelsResponse = childcareLevels.Select(
        childcareLevel => Map.FromEntity(
          childcareLevel)).ToList();

      var response = new APIResponse<IEnumerable<Response>>();
      response.IsSuccess = true;
      response.Data = childcareLevelsResponse;

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