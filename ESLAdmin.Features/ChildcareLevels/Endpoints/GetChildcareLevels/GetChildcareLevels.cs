using ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevel;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevels;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelsEndpoint
//
//------------------------------------------------------------------------------
public class GetChildcareLevelsEndpoint :
  EndpointWithoutRequest<
    Results<Ok<IEnumerable<GetChildcareLevelResponse>>, ProblemDetails, InternalServerError>,
    GetChildcareLevelMapper>
{
  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public GetChildcareLevelsEndpoint()
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Get("/api/childcarelevels/");
    //AuthSchemes("Bearer");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<IEnumerable<GetChildcareLevelResponse>>, ProblemDetails, InternalServerError>> ExecuteAsync(
    CancellationToken c)
  {
    var command = new GetChildcareLevelsCommand
    {
      Mapper = Map
    };

    return await command.ExecuteAsync(c);
  }
}