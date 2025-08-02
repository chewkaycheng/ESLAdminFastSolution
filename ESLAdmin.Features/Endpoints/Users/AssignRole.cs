using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        class AssignRoleEndpoint
//
//-------------------------------------------------------------------------------
public class AssignRoleEndpoint : Endpoint<
  AssignRoleRequest,
  Results<NoContent, ProblemDetails, InternalServerError>>
{
  private readonly ILogger<AssignRoleEndpoint> _logger;

  //------------------------------------------------------------------------------
  //
  //                       AssignRoleEndpoint
  //
  //-------------------------------------------------------------------------------
  public AssignRoleEndpoint(ILogger<AssignRoleEndpoint> logger)
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
    Post("/users/assign-role");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public override async Task<Results<NoContent, ProblemDetails, InternalServerError>>
  ExecuteAsync(AssignRoleRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var command = new AssignRoleCommand
      {
        Email = request.Email,
        RoleName = request.RoleName
      };
      return await command.ExecuteAsync(cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}
