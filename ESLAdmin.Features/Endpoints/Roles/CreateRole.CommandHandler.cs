using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;

//------------------------------------------------------------------------------
//
//                          class CreateRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class CreateRoleCommandHandler : ICommandHandler<
  CreateRoleCommand,
  Results<Ok<CreateRoleResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<CreateRoleCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                          CreateRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public CreateRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<CreateRoleCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                          ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<CreateRoleResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      CreateRoleCommand command,
      CancellationToken ct)
  {
    var result = await _repositoryManager.IdentityRepository.CreateRoleAsync(command.Name);

    if (result.IsError)
    {
      var error = result.Errors.First();
      var statusCode = StatusCodes.Status500InternalServerError;
      switch (error.Code)
      {
        case "Identity.DuplicateRoleName":
        case "Identity.InvalidRoleName":
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

    return TypedResults.Ok(command.Mapper.FromEntity(result.Value));
  }
}
