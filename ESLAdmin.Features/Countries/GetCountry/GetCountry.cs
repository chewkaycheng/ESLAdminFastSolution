using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Countries.GetCountry;

//------------------------------------------------------------------------------
//
//                        class GetCountryEndpoint
//
//------------------------------------------------------------------------------
public class GetCountryEndpoint :
  Endpoint<
    GetCountryRequest,
    Results<Ok<GetCountryResponse>, 
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
    Get("/api/Countries/{id}");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<GetCountryResponse>, 
    ProblemDetails, 
    InternalServerError>> ExecuteAsync(
    GetCountryRequest request, 
    CancellationToken c)
  {
    var command = new GetCountryCommand
    {
      Id = request.Id,
      Mapper = Map
    };

    return await command.ExecuteAsync(c);
  }
}