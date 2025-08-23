using ESLAdmin.Features.Identity.Endpoints.IdentityRoles.GetRole;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityRoles.GetRoles
{
  //------------------------------------------------------------------------------
  //
  //                        class GetRolesEndpoint
  //
  //------------------------------------------------------------------------------
  public class GetRolesEndpoint : EndpointWithoutRequest<
    Results<Ok<IEnumerable<GetRoleResponse>>, ProblemDetails, InternalServerError>,
    GetRoleMapper>
  {
    private readonly ILogger<GetRolesEndpoint> _logger;

    //------------------------------------------------------------------------------
    //
    //                        GetRolesEndpoint
    //
    //------------------------------------------------------------------------------
    public GetRolesEndpoint(ILogger<GetRolesEndpoint> logger)
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
      Get("/api/roles/");
      AllowAnonymous();
    }

    //------------------------------------------------------------------------------
    //
    //                        ExecuteAsync
    //
    //------------------------------------------------------------------------------
    public override async Task<Results<Ok<IEnumerable<GetRoleResponse>>, ProblemDetails, InternalServerError>> ExecuteAsync(
      CancellationToken c)
    {
      try
      {
        return await new GetRolesCommand
        {
          Mapper = Map
        }.ExecuteAsync(c);
      }
      catch (Exception ex)
      {
        _logger.LogException(ex);

        return TypedResults.InternalServerError();
      }
    }

  }
}
