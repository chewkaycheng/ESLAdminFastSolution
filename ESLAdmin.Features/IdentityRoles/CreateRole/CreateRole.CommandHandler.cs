using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityRoles.CreateRole;

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
    var result = await _repositoryManager
      .IdentityRepository
      .CreateRoleAsync(command.Name);

    if (!result.IsError)
      return TypedResults.Ok(command.Mapper.FromEntity(result.Value));

    var error = result.Errors.First();
    var statusCode = error.Code switch
    {
      "Identity.DuplicateRoleName" or "Identity.InvalidRoleName" 
        => StatusCodes.Status400BadRequest,
      "Identity.ConcurrencyError" => StatusCodes.Status409Conflict,
      "Database.OperationCanceled" => StatusCodes.Status400BadRequest,
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