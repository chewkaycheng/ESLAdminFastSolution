using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                        CreateChildcareLevelEndpoint
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelEndpoint : Endpoint<
  CreateChildcareLevelRequest,
  Results<NoContent, ProblemDetails, InternalServerError>,
  CreateChildcareLevelMapper>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly IMessageLogger _messageLogger;

  public CreateChildcareLevelEndpoint(
    IRepositoryManager  repositoryManager,
    IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _messageLogger = messageLogger;
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Post("/api/childcarelevels");
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<NoContent, ProblemDetails, InternalServerError>> ExecuteAsync(CreateChildcareLevelRequest request, CancellationToken canellationToken)
  {
    CreateChildcareLevelCommand command = new CreateChildcareLevelCommand
    {
      ChildcareLevelName = request.ChildcareLevelName,
      DisplayOrder = request.DisplayOrder,
      MaxCapacity = request.MaxCapacity,
      InitUser = request.InitUser,
      Mapper = Map
    };

    return await command.ExecuteAsync();
  }
}
