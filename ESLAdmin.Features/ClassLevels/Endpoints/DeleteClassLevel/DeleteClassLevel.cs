using ErrorOr;
using ESLAdmin.Features.ChildcareLevels.Endpoints.DeleteChildcareLevel;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ClassLevels.Endpoints.DeleteClassLevel;

//------------------------------------------------------------------------------
//
//                          class DeleteClassLevelEndpoint
//
//------------------------------------------------------------------------------
public class DeleteClassLevelEndpoint : Endpoint<
  DeleteClassLevelRequest,
  Results<Ok<Success>,
    ProblemDetails,
    InternalServerError>,
  DeleteClassLevelMapper>
{
  //------------------------------------------------------------------------------
  //
  //                          DeleteClassLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public DeleteClassLevelEndpoint()
  {
  }

  //------------------------------------------------------------------------------
  //
  //                           Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Delete("/api/classlevels/{id}");
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
        DeleteClassLevelRequest request,
        CancellationToken cancellationToken)
  {
    return await new DeleteClassLevelCommand
    {
      Id = request.Id,
      Mapper = Map
    }.ExecuteAsync(cancellationToken);
  }
}
