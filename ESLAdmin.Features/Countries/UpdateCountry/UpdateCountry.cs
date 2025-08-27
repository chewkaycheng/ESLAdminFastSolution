using ESLAdmin.Features.ClassLevels.Endpoints.UpdateClassLevel;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Countries.UpdateCountry;

//------------------------------------------------------------------------------
//
//                           class UpdateCountryEndpoint
//
//------------------------------------------------------------------------------
public class UpdateCountryEndpoint :
  Endpoint<UpdateCountryRequest,
    Results<Ok<UpdateCountryResponse>,
      ProblemDetails,
      InternalServerError>,
    UpdateCountryMapper>
{
  //------------------------------------------------------------------------------
  //
  //                           UpdateCountryEndpoint
  //
  //------------------------------------------------------------------------------
  public UpdateCountryEndpoint()
  {
  }

  //------------------------------------------------------------------------------
  //
  //                           Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Put("/api/countries");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                           ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<UpdateCountryResponse>,
    ProblemDetails,
    InternalServerError>> ExecuteAsync(
    UpdateCountryRequest request, 
    CancellationToken cancallationToken)
  {
    return await new UpdateCountryCommand
    {
      CountryId = request.CountryId,
      CountryName = request.CountryName,
      LanguageName = request.LanguageName,
      UserCode = request.UserCode,
      Guid = request.Guid,
      Mapper = Map
    }.ExecuteAsync();
  }

}
