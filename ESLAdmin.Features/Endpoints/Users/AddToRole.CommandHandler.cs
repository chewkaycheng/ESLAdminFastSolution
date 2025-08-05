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
//                       class AddToRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class AddToRoleCommandHandler : ICommandHandler<
  AddToRoleCommand,
  Results<NoContent, ProblemDetails, InternalServerError>>
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

  public async Task<Results<NoContent, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      AddToRoleCommand command,
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

    var result = await _repositoryManager.AuthenticationRepository.AddToRoleAsync(
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

        var statusCode = StatusCodes.Status404NotFound;
        if (error.Code == "User.UserAlreadyInRole")
        {
          statusCode = StatusCodes.Status400BadRequest;
        }

        var validationFailures = new List<ValidationFailure>
        {
          new ValidationFailure
          {
            PropertyName = error.Code,
            ErrorMessage = error.Description
          }
        };
        
        return new ProblemDetails(validationFailures, statusCode);
      }
    }

    _logger.LogFunctionExit($"Email: {command.Email}, RoleName: {command.RoleName}");
    return TypedResults.NoContent();
  }
}

