using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityRoles.DeleteRole;

//-------------------------------------------------------------------------------
//
//                       class DeleteRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class DeleteRoleCommandHandler : 
  IdentityCommandHandlerBase<DeleteRoleCommandHandler>,
  ICommandHandler<DeleteRoleCommand, Results<
    NoContent,
    ProblemDetails,
    InternalServerError>>
{
  //-------------------------------------------------------------------------------
  //
  //                       DeleteRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public DeleteRoleCommandHandler(
    IIdentityRepository repository,
    ILogger<DeleteRoleCommandHandler> logger) :
    base(repository, logger)
  {
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
    var result = await _repository
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