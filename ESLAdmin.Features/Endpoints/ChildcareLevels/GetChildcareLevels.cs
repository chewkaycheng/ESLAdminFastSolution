using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

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
  private readonly IRepositoryManager _manager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public GetChildcareLevelsEndpoint(
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
    Get("/api/childcarelevels/");
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