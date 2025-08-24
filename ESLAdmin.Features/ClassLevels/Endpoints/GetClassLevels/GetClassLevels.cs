using ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevel;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevels;

//------------------------------------------------------------------------------
//
//                        class GetClassLevelEndpoint
//
//------------------------------------------------------------------------------
public class GetClassLevelEndpoint :
  EndpointWithoutRequest<
    Results<Ok<IEnumerable<GetClassLevelResponse>>, 
      ProblemDetails, 
      InternalServerError>,
    GetClassLevelMapper>
{
  //------------------------------------------------------------------------------
  //
  //                        GetClassLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public GetClassLevelEndpoint()
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Get("/api/classlevels/");
    //AuthSchemes("Bearer");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<
    Results<Ok<IEnumerable<GetClassLevelResponse>>, 
      ProblemDetails, 
      InternalServerError>> ExecuteAsync(
    CancellationToken c)
  {
    var command = new GetClassLevelsCommand
    {
      Mapper = Map
    };

    return await command.ExecuteAsync(c);
  }
}