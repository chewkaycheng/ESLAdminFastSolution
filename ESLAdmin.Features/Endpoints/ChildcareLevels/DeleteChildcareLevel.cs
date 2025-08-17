using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels
{
  public class DeleteChildcareLevelEndpoint : Endpoint<
  DeleteChildcareLevelRequest,
  Results<NoContent, ProblemDetails, InternalServerError>,
  DeleteChildcareLevelMapper>
  {
    private readonly IRepositoryManager _manager;
    private readonly IMessageLogger _messageLogger;

    //------------------------------------------------------------------------------
    //
    //                           DeleteChildcareLevelEndpoint
    //
    //------------------------------------------------------------------------------
    public DeleteChildcareLevelEndpoint(
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
      Delete("/api/childcarelevels/{id}");
      AllowAnonymous();
    }

    //------------------------------------------------------------------------------
    //
    //                           ExecuteAsync
    //
    //------------------------------------------------------------------------------
    public override async Task<Results<NoContent, ProblemDetails, InternalServerError>> ExecuteAsync(
      DeleteChildcareLevelRequest request, CancellationToken cancellationToken)
    {
      return await new DeleteChildcareLevelCommand
      {
        Id = request.Id,
        Mapper = Map
      }.ExecuteAsync(cancellationToken);
    }
  }
}
