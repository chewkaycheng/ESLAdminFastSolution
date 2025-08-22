using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityRoles.CreateRole;

//------------------------------------------------------------------------------
//
//                        class CreateRoleEndpoint
//
//------------------------------------------------------------------------------
public class CreateRoleEndpoint :
  Endpoint<
    CreateRoleRequest,
    Results<Ok<CreateRoleResponse>, ProblemDetails, InternalServerError>,
    CreateRoleMapper>
{
  private readonly ILogger<CreateRoleEndpoint> _logger;

  //------------------------------------------------------------------------------
  //
  //                        CreateRoleEndpoint
  //
  //------------------------------------------------------------------------------
  public CreateRoleEndpoint(
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
    Post("/api/roles");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<CreateRoleResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(
    CreateRoleRequest request, CancellationToken c)
  {
    return await Map.ToCommand(request, Map).ExecuteAsync();
  }
}
