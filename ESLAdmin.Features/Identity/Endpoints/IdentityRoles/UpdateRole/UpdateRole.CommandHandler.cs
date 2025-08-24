using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityRoles.UpdateRole;

//------------------------------------------------------------------------------
//
//                       class UpdateRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class UpdateRoleCommandHandler : IdentityCommandHandlerBase<UpdateRoleCommandHandler>, 
  ICommandHandler<
    UpdateRoleCommand,
      Results<
        Ok<string>, 
        ProblemDetails, 
        InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                       UpdateRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public UpdateRoleCommandHandler(
    IIdentityRepository repository,
    ILogger<UpdateRoleCommandHandler> logger) :
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<string>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      UpdateRoleCommand command,
      CancellationToken ct)
  {
    var result = await _repository
      .UpdateRoleAsync(command.OldName, command.NewName);

    if (!result.IsError)
      return TypedResults.Ok(result.Value);
 
    var error = result.Errors.First();
    var statusCode = error.Code switch
    {
      "Identity.RoleNotFound"
      or "Identity.InvalidRoleName"
      or "Identity.ConcurrencyError"
      or "Identity.RoleAlreadyExists" 
        => StatusCodes.Status400BadRequest,
      _ => StatusCodes.Status500InternalServerError
    };

    return new ProblemDetails(
      ErrorUtils.CreateFailureList(
        error.Code,
        error.Description),
      statusCode);
  }
}