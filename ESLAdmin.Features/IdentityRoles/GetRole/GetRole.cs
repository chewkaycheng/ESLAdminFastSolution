using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityRoles.GetRole;

//-------------------------------------------------------------------------------
//
//                       class GetRoleEndpoint
//
//-------------------------------------------------------------------------------
public class GetRoleEndpoint : Endpoint<
  GetRoleRequest,
  Results<Ok<GetRoleResponse>, ProblemDetails, InternalServerError>,
  GetRoleMapper>
{
  private readonly ILogger<GetRoleEndpoint> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       GetRoleEndpoint
  //
  //-------------------------------------------------------------------------------
  public GetRoleEndpoint(ILogger<GetRoleEndpoint> logger)
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
    Get("/api/roles/{name}");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<GetRoleResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(
    GetRoleRequest request, CancellationToken c)
  {
    try
    {
      return await new GetRoleCommand
      {
        Name = request.Name,
        Mapper = Map
      }.ExecuteAsync();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}
