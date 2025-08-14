using ESLAdmin.Common.Errors;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;

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
    try
    {
      var result = await _repositoryManager.IdentityRepository.UpdateRoleAsync(command.OldName, command.NewName);

      if (result.IsError)
      {
        var error = result.Errors.First();
        var statusCode = StatusCodes.Status500InternalServerError;
        switch (error.Code)
        {
          case "Identity.RoleNotFound":
          case "Identity.InvalidRoleName":
            statusCode = StatusCodes.Status400BadRequest;
            break;
          case "Identity.ConcurrencyError":
          case "Identity.RoleAlreadyExists":
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

      return TypedResults.Ok(result.Value);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}
