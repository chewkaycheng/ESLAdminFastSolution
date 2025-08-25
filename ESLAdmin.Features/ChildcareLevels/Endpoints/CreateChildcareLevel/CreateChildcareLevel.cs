using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.CreateChildcareLevel;

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
  //private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<CreateChildcareLevelEndpoint> _logger;

  public CreateChildcareLevelEndpoint(
    //IRepositoryManager repositoryManager,
    ILogger<CreateChildcareLevelEndpoint> logger)
  {
    //_repositoryManager = repositoryManager;
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
    CancellationToken cancellationToken)
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

    var result = await command.ExecuteAsync(cancellationToken);

    if (result.Result is Ok<CreateChildcareLevelResponse> { Value: not null } okResult)
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
      var context = $"\n=>Request: \n    ChildcareLevelName: '{request.ChildcareLevelName}', \n    DisplayOrder: '{request.DisplayOrder}', \n    MaxCapacity: '{request.MaxCapacity}', \n    InitUser: '{request.InitUser}'";
      _logger.LogFunctionEntry(context);
    }
  }

}
