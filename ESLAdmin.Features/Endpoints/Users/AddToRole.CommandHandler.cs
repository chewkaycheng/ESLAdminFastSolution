using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class AddToRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class AddToRoleCommandHandler : ICommandHandler<
  AddToRoleCommand,
  Results<Ok<Success>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<AddToRoleCommandHandler> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       AddToRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public AddToRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<AddToRoleCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
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

    var result = await _repositoryManager.IdentityRepository.AddToRoleAsync(
      command.Email,
      command.RoleName);

    if (result.IsError)
    {
      var error = result.Errors.First();
      if (error.Code.Contains("Exception"))
      {
        return TypedResults.InternalServerError();
      }
      var statusCode = error.Code switch
      {
        "Database.ConcurrencyFailure" => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
      };

      return new ProblemDetails(
        ErrorUtils.CreateFailureList(
          error.Code, 
          error.Description),
          statusCode);
    }

    _logger.LogFunctionExit(
      $"Email: {command.Email}, RoleName: {command.RoleName}");
    return TypedResults.Ok(new Success());
  }
}

