using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                        CreateChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelEndpoint : Endpoint<
  CreateChildcareLevelRequest,
  Results<Ok<CreateChildcareLevelResponse>, ProblemDetails, InternalServerError>,
  CreateChildcareLevelMapper>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<CreateChildcareLevelEndpoint> _logger;

  public CreateChildcareLevelEndpoint(
    IRepositoryManager repositoryManager,
    ILogger<CreateChildcareLevelEndpoint> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Post("/api/childcarelevels");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<CreateChildcareLevelResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(
    CreateChildcareLevelRequest request,
    CancellationToken canellationToken)
  {
    DebugLogFunctionEntry(request);

    CreateChildcareLevelCommand command = new CreateChildcareLevelCommand
    {
      ChildcareLevelName = request.ChildcareLevelName,
      DisplayOrder = request.DisplayOrder,
      MaxCapacity = request.MaxCapacity,
      InitUser = request.InitUser,
      Mapper = Map
    };

    var result = await command.ExecuteAsync();

    if (result.Result is Ok<CreateChildcareLevelResponse> okResult && okResult != null && okResult.Value != null)
    {
      HttpContext.Response.Headers.Append("location", $"/api/childcarelevels/{okResult.Value.ChildcareLevelId}");
    }

    _logger.LogFunctionExit();
    return result;
  }

  //------------------------------------------------------------------------------
  //
  //                        DebugLogFunctionEntry
  //
  //------------------------------------------------------------------------------
  private void DebugLogFunctionEntry(CreateChildcareLevelRequest request)
  {
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      var context = $"\n=>Request: \n    ChildcareLevelName: '{request.ChildcareLevelName}', DisplayOrder: '{request.DisplayOrder}', MaxCapacity: '{request.MaxCapacity}', InitUser: '{request.InitUser}'";
      _logger.LogFunctionEntry(context);
    }
  }

}
