using ErrorOr;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.AddToRole;

//------------------------------------------------------------------------------
//
//                        class AddToRoleEndpoint
//
//-------------------------------------------------------------------------------
public class AddToRoleEndpoint : Endpoint<
  AddToRoleRequest,
  Results<Ok<Success>, ProblemDetails, InternalServerError>>
{
  private readonly ILogger<AddToRoleEndpoint> _logger;

  //------------------------------------------------------------------------------
  //
  //                       AddToRoleEndpoint
  //
  //-------------------------------------------------------------------------------
  public AddToRoleEndpoint(ILogger<AddToRoleEndpoint> logger)
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
    Post("/api/users/add-to-role");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public override async Task<Results<Ok<Success>, ProblemDetails, InternalServerError>>
    ExecuteAsync(AddToRoleRequest request, CancellationToken cancellationToken)
  {
    return await new AddToRoleCommand
    {
      Email = request.Email,
      RoleName = request.RoleName
    }.ExecuteAsync(cancellationToken);
  }
}