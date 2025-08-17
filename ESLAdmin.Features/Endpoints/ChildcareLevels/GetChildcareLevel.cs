using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class GetChildcareLevelEndpoint :
  Endpoint<
    GetChildcareLevelRequest,
    Results<Ok<GetChildcareLevelResponse>, ProblemDetails, InternalServerError>,
    GetChildcareLevelMapper>
{
  private readonly IRepositoryManager _manager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public GetChildcareLevelEndpoint(
    IRepositoryManager manager,
    IMessageLogger messageLogger)
  {
    _manager = manager;
    _messageLogger = messageLogger;
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Get("/api/childcarelevels/{id}");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<GetChildcareLevelResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(
    GetChildcareLevelRequest request, CancellationToken c)
  {
    var command = new GetChildcareLevelCommand
    {
      Id = request.Id,
      Mapper = Map
    };

    return await command.ExecuteAsync(c);
  }
}