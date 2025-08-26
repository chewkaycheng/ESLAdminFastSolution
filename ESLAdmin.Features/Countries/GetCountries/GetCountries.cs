using ESLAdmin.Features.Countries.GetCountry;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Countries.GetCountries;

//------------------------------------------------------------------------------
//
//                        class GetCountryEndpoint
//
//------------------------------------------------------------------------------
public class GetCountryEndpoint :
  EndpointWithoutRequest<
    Results<Ok<IEnumerable<GetCountryResponse>>, 
      ProblemDetails, 
      InternalServerError>,
    GetCountryMapper>
{
  //------------------------------------------------------------------------------
  //
  //                        GetCountryEndpoint
  //
  //------------------------------------------------------------------------------
  public GetCountryEndpoint()
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Get("/api/Countries/");
    //AuthSchemes("Bearer");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<
    Results<Ok<IEnumerable<GetCountryResponse>>, 
      ProblemDetails, 
      InternalServerError>> ExecuteAsync(
    CancellationToken c)
  {
    var command = new GetCountriesCommand
    {
      Mapper = Map
    };

    return await command.ExecuteAsync(c);
  }
}