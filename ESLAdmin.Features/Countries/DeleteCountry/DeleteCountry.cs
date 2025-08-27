using ErrorOr;
using ESLAdmin.Features.ClassLevels.Endpoints.DeleteClassLevel;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Countries.DeleteCountry;

//------------------------------------------------------------------------------
//
//                          class DeleteCountryEndpoint
//
//------------------------------------------------------------------------------
public class DeleteCountryEndpoint : Endpoint<
  DeleteCountryRequest,
  Results<Ok<Success>,
    ProblemDetails,
    InternalServerError>,
  DeleteCountryMapper>
{
  //------------------------------------------------------------------------------
  //
  //                          DeleteCountryEndpoint
  //
  //------------------------------------------------------------------------------
  public DeleteCountryEndpoint()
  {
  }

  //------------------------------------------------------------------------------
  //
  //                           Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Delete("/api/countries/{id}");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                           ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<Success>,
    ProblemDetails,
    InternalServerError>>
      ExecuteAsync(
        DeleteCountryRequest request,
        CancellationToken cancellationToken)
  {
    return await new DeleteCountryCommand
    {
      Id = request.Id,
      Mapper = Map
    }.ExecuteAsync(cancellationToken);
  }
}
