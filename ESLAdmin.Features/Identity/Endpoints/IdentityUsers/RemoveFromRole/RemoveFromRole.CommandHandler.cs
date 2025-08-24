using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Endpoints.IdentityRoles;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.RemoveFromRole;

//-------------------------------------------------------------------------------
//
//                       class RemoveFromRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class RemoveFromRoleCommandHandler : 
  IdentityCommandHandlerBase<RemoveFromRoleCommandHandler>, 
  ICommandHandler<RemoveFromRoleCommand,
    Results<NoContent, ProblemDetails, InternalServerError>>
{
  //-------------------------------------------------------------------------------
  //
  //                       RemoveFromRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public RemoveFromRoleCommandHandler(
    IIdentityRepository repository,
    ILogger<RemoveFromRoleCommandHandler> logger) :
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
      RemoveFromRoleCommand command,
      CancellationToken cancellationToken)
  {
    _logger.LogFunctionEntry($"Email: {command.Email}, RoleName: {command.RoleName}");

    var result = await _repository
      .RemoveFromRoleAsync(
        command.Email,
        command.RoleName);

    if (result.IsError)
    {
      var error = result.Errors.First();
      var statusCode = error.Code switch
      {
        "Identity.ConcurrencyFailure" => StatusCodes.Status409Conflict,
        string code when code.Contains("Exception") => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status404NotFound
      };

      return AppErrors.ProblemDetailsFactory.CreateProblemDetails(
        error.Code,
        error.Description,
        statusCode);
    }

    _logger.LogFunctionExit($"Email: {command.Email}, RoleName: {command.RoleName}");
    return TypedResults.NoContent();
  }
}