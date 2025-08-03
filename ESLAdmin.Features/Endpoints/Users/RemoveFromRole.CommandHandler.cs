using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class RemoveFromRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class RemoveFromRoleCommandHandler : ICommandHandler<
  RemoveFromRoleCommand,
  Results<NoContent, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<RemoveFromRoleCommandHandler> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       AddToRoleRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public RemoveFromRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<RemoveFromRoleCommandHandler> logger)
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
      RemoveFromRoleCommand command,
      CancellationToken cancellationToken)
  {
    _logger.LogFunctionEntry($"Email: {command.Email}, RoleName: {command.RoleName}");
    if (string.IsNullOrEmpty(command.Email) || string.IsNullOrEmpty(command.RoleName))
    {
      var validationFailures = new List<ValidationFailure>
      {
        new ValidationFailure("Email", "Email cannot be null or empty."),
        new ValidationFailure("RoleName", "RoleName cannot be null or empty.")
      };
      return new ProblemDetails(validationFailures, StatusCodes.Status400BadRequest);
    }

    var result = await _repositoryManager.AuthenticationRepository.RemoveFromRoleAsync(
      command.Email,
      command.RoleName);

    if (result.IsError)
    {
      foreach (var error in result.Errors)
      {
        if (error.Code == "Exception")

        {
          return TypedResults.InternalServerError();
        }

        var validationFailures = new List<ValidationFailure>
        {
          new ValidationFailure
          {
            PropertyName = error.Code,
            ErrorMessage = error.Description
          }
        };
        if (error.Code == "User.UserAlreadyInRole")
        {
          return new ProblemDetails(validationFailures, StatusCodes.Status400BadRequest);
        }
        return new ProblemDetails(validationFailures, StatusCodes.Status404NotFound);
      }
    }

    _logger.LogFunctionExit($"Email: {command.Email}, RoleName: {command.RoleName}");
    return TypedResults.NoContent();
  }
}

