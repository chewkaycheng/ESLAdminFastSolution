using Dapper;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using System.Collections.Generic;

namespace ESLAdmin.Features.ChildcareLevels.GetChildcareLevel;

public class Endpoint : Endpoint<EmptyRequest, APIResponse<Response>, Mapper>
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
    Get("/api/childcarelevels/{id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(EmptyRequest req, CancellationToken c)
  {
    try
    {
      var id = Route<int>("id");
      var sql = DbConstsChildcareLevel.SQL_GETBYID;

      DynamicParameters parameters = new DynamicParameters();
      parameters.AddInt64InputParam(
        OperationResultConsts.ID,
        id);
      
      ChildcareLevel? childcareLevel = await _repository.DapQuerySingleAsync(
        sql,
        parameters);
      if (childcareLevel == null)
      {
        var apiResponse = new APIResponse<Response>();
        apiResponse.IsSuccess = false;
        apiResponse.Error = $"A childcare level with Id: {id} does not exist.";
        await SendAsync(apiResponse, 404, c);
      }
      else
      {
        Response childcareLevelResponse = Map.FromEntity(childcareLevel);
        var apiResponse = new APIResponse<Response>();
        apiResponse.IsSuccess = true;
        apiResponse.Data = childcareLevelResponse;
        await SendAsync(apiResponse);
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(HandleAsync), ex);

      var apiResponse = new APIResponse<Response>();

      apiResponse.IsSuccess = false;
      apiResponse.Error = "Internal Server Error";
      await SendAsync(apiResponse, 500, c);
    }
  }
}