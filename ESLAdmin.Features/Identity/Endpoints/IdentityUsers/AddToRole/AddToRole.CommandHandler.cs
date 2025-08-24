using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Endpoints.IdentityRoles;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.AddToRole;

//-------------------------------------------------------------------------------
//
//                       class AddToRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class AddToRoleCommandHandler : IdentityCommandHandlerBase<AddToRoleCommandHandler>, 
  ICommandHandler<
    AddToRoleCommand,
      Results<
        Ok<Success>, 
        ProblemDetails, 
        InternalServerError>>
{
  //-------------------------------------------------------------------------------
  //
  //                       AddToRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public AddToRoleCommandHandler(
    IIdentityRepository repository,
    ILogger<AddToRoleCommandHandler> logger) :
    base(repository, logger)
  {
  }

  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------

  public async Task<Results<Ok<Success>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      AddToRoleCommand command,
      CancellationToken cancellationToken)
  {
    _logger.LogFunctionEntry($"Email: {command.Email}, RoleName: {command.RoleName}");

    var result = await _repository
      .AddToRoleAsync(
        command.Email,
        command.RoleName);

    if (result.IsError)
    {
      var error = result.Errors.First();
      var statusCode = error.Code switch
      {
        "Database.ConcurrencyFailure" => StatusCodes.Status409Conflict,
        "Database.OperationCanceled" => StatusCodes.Status408RequestTimeout,
        string code when code.Contains("Exception") => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest
      };
      
      return AppErrors
        .ProblemDetailsFactory
        .CreateProblemDetails(
          error.Code,
          error.Description,
          statusCode,
          error.Code == "Identity.AddToRoleFailed" ? error.Metadata : null);
    }

    _logger.LogFunctionExit(
      $"Email: {command.Email}, RoleName: {command.RoleName}");
    return TypedResults.Ok(new Success());
  }
}