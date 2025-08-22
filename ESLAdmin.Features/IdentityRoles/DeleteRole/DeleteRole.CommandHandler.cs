using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityRoles.DeleteRole;

//-------------------------------------------------------------------------------
//
//                       class DeleteRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class DeleteRoleCommandHandler : ICommandHandler<
  DeleteRoleCommand,
  Results<
    NoContent,
    ProblemDetails,
    InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<DeleteRoleCommandHandler> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       DeleteRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public DeleteRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<DeleteRoleCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<NoContent, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      DeleteRoleCommand command,
      CancellationToken ct)
  {
    var result = await _repositoryManager
      .IdentityRepository
      .DeleteRoleAsync(command.Name);
    if (!result.IsError)
      return TypedResults.NoContent();

    var error = result.Errors.First();
    var statusCode = error.Code switch
    {
      "Identity.RoleNotFound" => StatusCodes.Status400BadRequest,
      "Identity.ConcurrencyError" => StatusCodes.Status409Conflict,
      _ => StatusCodes.Status500InternalServerError
    };

    return AppErrors
      .ProblemDetailsFactory
      .CreateProblemDetails(
        error.Code,
        error.Description,
        statusCode);
  }
}