using Dapper;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.CreateChildcareLevel;

public class Endpoint : Endpoint<Request, APIResponse<OperationResult>, Mapper>
{
  private readonly IRepositoryBase<Request, OperationResult> _repository;
  private readonly IMessageLogger _messageLogger;
  public Endpoint(
    IRepositoryBase<Request, OperationResult> repository,
    IMessageLogger messageLogger)
  {
    _repository = repository;
    _messageLogger = messageLogger;
  }

  public override void Configure()
  {
    Post("/api/childcarelevels");
  }

  public override async Task HandleAsync(Request r, CancellationToken c)
  {
    try
    {
      DynamicParameters parameters = Map.ToEntity(r);
      var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_ADD;

      await _repository.DapExecWithTransAsync(sql, parameters);
      var result = Map.FromEntity(parameters);

      if (result.DbApiError == 0)
      {
        var response = new APIResponse<OperationResult>();
        response.IsSuccess = true;
        response.Data = result;
      }
      else
      {
        var response = new APIResponse<OperationResult>();

        response.IsSuccess = false;
        response.Error = $"Another record with the childcare level name: {r.ChildcareLevelName} already exists.";
        await SendAsync(response, 409, c);
      }
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