using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelEndpoint : Endpoint<
  UpdateChildcareLevelRequest,
  Results<Ok<UpdateChildcareLevelResponse>,
    ProblemDetails,
    InternalServerError>,
  UpdateChildcareLevelMapper>
{
  private readonly IRepositoryManager _manager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                           UpdateChildcareLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public UpdateChildcareLevelEndpoint(
    IRepositoryManager manager,
    IMessageLogger messageLogger)
  {
    _manager = manager;
    _messageLogger = messageLogger;
  }

  //------------------------------------------------------------------------------
  //
  //                           Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Put("/api/childcarelevels");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                           ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<UpdateChildcareLevelResponse>,
    ProblemDetails,
    InternalServerError>> ExecuteAsync(
      UpdateChildcareLevelRequest request, CancellationToken cancallationToken)
  {
    return await new UpdateChildcareLevelCommand
    {
      ChildcareLevelId = request.ChildcareLevelId,
      ChildcareLevelName = request.ChildcareLevelName,
      MaxCapacity = request.MaxCapacity,
      DisplayOrder = request.DisplayOrder,
      UserCode = request.UserCode,
      Guid = request.Guid,
      Mapper = Map
    }.ExecuteAsync();
  }
}
