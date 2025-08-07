using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        class AddToRoleEndpoint
//
//-------------------------------------------------------------------------------
public class AddToRoleEndpoint : Endpoint<
  AddToRoleRequest,
  Results<NoContent, ProblemDetails, InternalServerError>>
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
  public override async Task<Results<NoContent, ProblemDetails, InternalServerError>>
  ExecuteAsync(AddToRoleRequest request, CancellationToken cancellationToken)
  {
    return await new AddToRoleCommand
    {
      Email = request.Email,
      RoleName = request.RoleName
    }.ExecuteAsync(cancellationToken);
  }
}
