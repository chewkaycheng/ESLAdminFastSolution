using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ClassLevels.Endpoints.UpdateClassLevel;

//------------------------------------------------------------------------------
//
//                           class UpdateClassLevelEndpoint
//
//------------------------------------------------------------------------------
public class UpdateClassLevelEndpoint : Endpoint<UpdateClassLevelRequest,
  Results<Ok<UpdateClassLevelResponse>,
    ProblemDetails,
    InternalServerError>,
  UpdateClassLevelMapper>
{
  //------------------------------------------------------------------------------
  //
  //                           UpdateClassLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public UpdateClassLevelEndpoint()
  {
    
  }
  
  //------------------------------------------------------------------------------
  //
  //                           Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Put("/api/classlevels");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                           ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<UpdateClassLevelResponse>,
    ProblemDetails,
    InternalServerError>> ExecuteAsync(
    UpdateClassLevelRequest request, CancellationToken cancallationToken)
  {
    return await new UpdateClassLevelCommand
    {
      ClassLevelId = request.ClassLevelId,
      ClassLevelName = request.ClassLevelName,
      DisplayOrder = request.DisplayOrder,
      DisplayColor = request.DisplayColor,
      UserCode = request.UserCode,
      Guid = request.Guid,
      Mapper = Map
    }.ExecuteAsync();
  }
}