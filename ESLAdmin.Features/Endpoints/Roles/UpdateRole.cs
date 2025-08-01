using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;

//------------------------------------------------------------------------------
//
//                        class UpdateRoleEndpoint
//
//------------------------------------------------------------------------------
public class UpdateRoleEndpoint :
  Endpoint<UpdateRoleRequest,
    Results<Ok<string>, ProblemDetails, InternalServerError>>
{
  private readonly ILogger<CreateRoleEndpoint> _logger;
  public UpdateRoleEndpoint(ILogger<CreateRoleEndpoint> logger)
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
    Put("/api/roles");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<string>, ProblemDetails, InternalServerError>> 
    ExecuteAsync(
      UpdateRoleRequest request, 
      CancellationToken c)
  {
    try
    {
      UpdateRoleCommand command = new UpdateRoleCommand
      {
        OldName = request.OldName,
        NewName = request.NewName
      };
      return await command.ExecuteAsync(c);

    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }

  }
}
