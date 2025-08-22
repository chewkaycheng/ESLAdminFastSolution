using ESLAdmin.Features.IdentityUsers.AddToRole;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityUsers.RemoveFromRole;

//------------------------------------------------------------------------------
//
//                        class RemoveFromRoleEndpoint
//
//-------------------------------------------------------------------------------
public class RemoveFromRoleEndpoint : Endpoint<
  RemoveFromRoleRequest,
  Results<NoContent, ProblemDetails, InternalServerError>>
{
  private readonly ILogger<AddToRoleEndpoint> _logger;

  //------------------------------------------------------------------------------
  //
  //                       RemoveFromRoleEndpoint
  //
  //-------------------------------------------------------------------------------
  public RemoveFromRoleEndpoint(ILogger<AddToRoleEndpoint> logger)
  {
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                       Configure
  //
  //-------------------------------------------------------------------------------
  public override void Configure()
  {
    Post("/api/users/remove-from-role");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public override async Task<Results<NoContent, ProblemDetails, InternalServerError>>
  ExecuteAsync(RemoveFromRoleRequest request, CancellationToken cancellationToken)
  {
    return await new RemoveFromRoleCommand
    {
      Email = request.Email,
      RoleName = request.RoleName
    }.ExecuteAsync(cancellationToken);
  }
}
