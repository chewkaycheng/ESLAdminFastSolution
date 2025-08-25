using ErrorOr;
using ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Repositories;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.DeleteChildcareLevel
{
  //------------------------------------------------------------------------------
  //
  //                        class DeleteChildcareLevelEndpoint
  //
  //------------------------------------------------------------------------------
  public class DeleteChildcareLevelEndpoint : Endpoint<
    DeleteChildcareLevelRequest,
    Results<Ok<Success>, 
      ProblemDetails, 
      InternalServerError>,
    DeleteChildcareLevelMapper>
  {
    private readonly IChildcareLevelRepository _repository;
    private readonly IMessageLogger _messageLogger;

    //------------------------------------------------------------------------------
    //
    //                           DeleteChildcareLevelEndpoint
    //
    //------------------------------------------------------------------------------
    public DeleteChildcareLevelEndpoint(
      IChildcareLevelRepository repository,
      IMessageLogger messageLogger)
    {
      _repository = repository;
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
    public override async Task<Results<Ok<Success>, 
      ProblemDetails, 
      InternalServerError>> 
        ExecuteAsync(
          DeleteChildcareLevelRequest request, 
          CancellationToken cancellationToken)
    {
      return await new DeleteChildcareLevelCommand
      {
        Id = request.Id,
        Mapper = Map
      }.ExecuteAsync(cancellationToken);
    }
  }
}
