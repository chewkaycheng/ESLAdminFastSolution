using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityRoles.CreateRole;

//------------------------------------------------------------------------------
//
//                          class CreateRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class CreateRoleCommandHandler :
  IdentityCommandHandlerBase<CreateRoleCommandHandler>,
  ICommandHandler<
  CreateRoleCommand,
    Results<Ok<CreateRoleResponse>, ProblemDetails, InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                          CreateRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public CreateRoleCommandHandler(
    IIdentityRepository repository,
    ILogger<CreateRoleCommandHandler> logger) :
    base(repository, logger)
  {
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
    var result = await _repository
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