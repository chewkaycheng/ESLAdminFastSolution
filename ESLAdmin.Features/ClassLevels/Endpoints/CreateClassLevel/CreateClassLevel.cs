using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ClassLevels.Endpoints.CreateClassLevel;

//------------------------------------------------------------------------------
//
//                        CreateClassLevelEndpoint
//
//------------------------------------------------------------------------------
public class CreateClassLevelEndpoint : Endpoint<
  CreateClassLevelRequest,
  Results<Ok<CreateClassLevelResponse>, 
    ProblemDetails, 
    InternalServerError>,
  CreateClassLevelMapper>
{
  //private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<CreateClassLevelEndpoint> _logger;

  public CreateClassLevelEndpoint(
    //IRepositoryManager repositoryManager,
    ILogger<CreateClassLevelEndpoint> logger)
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
    Post("/api/Classlevels");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<CreateClassLevelResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(
    CreateClassLevelRequest request,
    CancellationToken cancellationToken)
  {
    DebugLogFunctionEntry(request);

    CreateClassLevelCommand command = new CreateClassLevelCommand
    {
      ClassLevelName = request.ClassLevelName,
      DisplayOrder = request.DisplayOrder,
      DisplayColor = request.DisplayColor,
      InitUser = request.InitUser,
      Mapper = Map
    };

    var result = await command.ExecuteAsync(cancellationToken);

    if (result.Result is Ok<CreateClassLevelResponse> { Value: not null } okResult)
    {
      HttpContext.Response.Headers.Append("location", $"/api/Classlevels/{okResult.Value.ClassLevelId}");
    }

    _logger.LogFunctionExit();
    return result;
  }

  //------------------------------------------------------------------------------
  //
  //                        DebugLogFunctionEntry
  //
  //------------------------------------------------------------------------------
  private void DebugLogFunctionEntry(CreateClassLevelRequest request)
  {
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      var context = $"\n=>Request: \n    ClassLevelName: '{request.ClassLevelName}', \n    DisplayOrder: '{request.DisplayOrder}', \n    DisplayColor: '{request.DisplayColor}', \n    InitUser: '{request.InitUser}'";
      _logger.LogFunctionEntry(context);
    }
  }

}
