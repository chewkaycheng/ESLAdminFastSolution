using Dapper;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using System.Collections.Generic;

namespace ESLAdmin.Features.ChildcareLevels.GetChildcareLevel;

public class Endpoint : Endpoint<EmptyRequest, APIResponse<Response>, Mapper>
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
    Get("/api/childcarelevels/{id}");
    AllowAnonymous();
  }

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

      var apiResponse = new APIResponse<Response>();

      apiResponse.IsSuccess = false;
      apiResponse.Error = "Internal Server Error";
      await SendAsync(apiResponse, 500, c);
    }
  }
}