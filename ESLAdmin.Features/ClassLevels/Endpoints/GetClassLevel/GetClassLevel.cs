using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevel;

//------------------------------------------------------------------------------
//
//                        class GetClassLevelEndpoint
//
//------------------------------------------------------------------------------
public class GetClassLevelEndpoint :
  Endpoint<
    GetClassLevelRequest,
    Results<Ok<GetClassLevelResponse>, 
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
    Get("/api/classlevels/{id}");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<GetClassLevelResponse>, 
    ProblemDetails, 
    InternalServerError>> ExecuteAsync(
      GetClassLevelRequest request, 
      CancellationToken c)
  {
    var command = new GetClassLevelCommand
    {
      Id = request.Id,
      Mapper = Map
    };

    return await command.ExecuteAsync(c);
  }
}