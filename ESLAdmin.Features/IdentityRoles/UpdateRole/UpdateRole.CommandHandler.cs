using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityRoles.UpdateRole;

//------------------------------------------------------------------------------
//
//                       class UpdateRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class UpdateRoleCommandHandler : ICommandHandler<
  UpdateRoleCommand,
  Results<Ok<string>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<UpdateRoleCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                       UpdateRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public UpdateRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<UpdateRoleCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
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
    var result = await _repositoryManager
      .IdentityRepository
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