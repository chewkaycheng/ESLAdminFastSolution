using ESLAdmin.Features.ClassLevels.Endpoints.CreateClassLevel;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Countries.CreateCountry;

//------------------------------------------------------------------------------
//
//                        CreateCountryEndpoint
//
//------------------------------------------------------------------------------
public class CreateCountryEndpoint : Endpoint<
  CreateCountryRequest,
  Results<Ok<CreateCountryResponse>,
    ProblemDetails,
    InternalServerError>,
  CreateCountryMapper>
{
  private readonly ILogger<CreateCountryEndpoint> _logger;

  public CreateCountryEndpoint(
    ILogger<CreateCountryEndpoint> logger)
  {
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Post("/api/Countries");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<CreateCountryResponse>, 
    ProblemDetails, 
    InternalServerError>> 
      ExecuteAsync(
        CreateCountryRequest request,
        CancellationToken cancellationToken)
  {
    DebugLogFunctionEntry(request);

    CreateCountryCommand command = new CreateCountryCommand
    {
      CountryName = request.CountryName,
      LanguageName = request.LanguageName,
      InitUser = request.InitUser,
      Mapper = Map
    };

    var result = await command.ExecuteAsync(cancellationToken);

    if (result.Result is Ok<CreateCountryResponse> { Value: not null } okResult)
    {
      HttpContext.Response.Headers.Append("location", $"/api/Countries/{okResult.Value.CountryId}");
    }

    _logger.LogFunctionExit();
    return result;
  }

  //------------------------------------------------------------------------------
  //
  //                        DebugLogFunctionEntry
  //
  //------------------------------------------------------------------------------
  private void DebugLogFunctionEntry(CreateCountryRequest request)
  {
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      var context = $"\n=>Request: \n    CountryName: '{request.CountryName}', \n    LanguageName: '{request.LanguageName}', \n    InitUser: '{request.InitUser}'";
      _logger.LogFunctionEntry(context);
    }
  }
}