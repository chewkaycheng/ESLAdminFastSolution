using ESLAdmin.Features.Endpoints.ChildcareLevels;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;

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
    try
    {
      var command = Map.ToCommand(request, Map); ;

      return await command.ExecuteAsync(c);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}
