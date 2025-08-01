using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles
{
  //------------------------------------------------------------------------------
  //
  //                        DeleteRoleEndpoint
  //
  //------------------------------------------------------------------------------
  public class DeleteRoleEndpoint : Endpoint<
    DeleteRoleRequest,
    Results<NoContent, ProblemDetails, InternalServerError>>
  {
    private readonly ILogger<CreateRoleEndpoint> _logger;

    //------------------------------------------------------------------------------
    //
    //                        DeleteRoleEndpoint
    //
    //------------------------------------------------------------------------------
    public DeleteRoleEndpoint(
      ILogger<CreateRoleEndpoint> logger)
    {
      _logger = logger;
    }

    //------------------------------------------------------------------------------
    //
    //                        Configure
    //
    //------------------------------------------------------------------------------
    public override void Configure()
    {
      Delete("/api/roles/{name}");
      AllowAnonymous();
    }

    //------------------------------------------------------------------------------
    //
    //                        ExecuteAsync
    //
    //------------------------------------------------------------------------------
    public override async Task<Results<NoContent, ProblemDetails, InternalServerError>>
      ExecuteAsync(
        DeleteRoleRequest request,
        CancellationToken c)
    {
      try
      {
        return await new DeleteRoleCommand
        { 
          Name = request.Name 
        }.ExecuteAsync();
      }
      catch (Exception ex)
      {
        _logger.LogException(ex);

        return TypedResults.InternalServerError();
      }
    }
  }
}
