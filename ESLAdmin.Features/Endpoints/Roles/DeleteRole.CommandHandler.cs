using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;

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
    try
    {
      var result = await _repositoryManager.IdentityRepository.DeleteRoleAsync(command.Name);
      if (result.IsError)
      {
        var error = result.Errors.First();
        var statusCode = StatusCodes.Status500InternalServerError;
        switch (error.Code)
        {
          case "Identity.RoleNotFound":
            statusCode = StatusCodes.Status400BadRequest;
            break;
          case "Identity.ConcurrencyError":
            statusCode = StatusCodes.Status409Conflict;
            break;
          default:
            return TypedResults.InternalServerError();
        }

        return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            error.Code,
            error.Description),
          statusCode);
      }

      return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }

  }
}
