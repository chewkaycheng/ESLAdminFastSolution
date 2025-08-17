using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                       class RegisterUserCommandHandler
//
//-------------------------------------------------------------------------------
public class RegisterUserCommandHandler : ICommandHandler<
      RegisterUserCommand,
      Results<NoContent, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<RegisterUserCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                       RegisterUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public RegisterUserCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<RegisterUserCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<NoContent, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      RegisterUserCommand command,
      CancellationToken cancellationToken)
  {
    try
    {
      var user = command.Mapper.CommandToEntity(command);

      var result = await _repositoryManager.IdentityRepository.RegisterUserAsync(
        user,
        command.Password,
        command.Roles);

      if (result.IsError)
      {
        var error = result.Errors.First();
        var statusCode = StatusCodes.Status500InternalServerError;
        switch (error.Code)
        {
          case "Identity.UserNotFound":
          case "Identity.RoleNotFound":
            statusCode = StatusCodes.Status404NotFound;
            break;
          case "Identity.UserAlreadyInRole":
          case "Identity.DuplicateUserName":
          case "Identity.DuplicateEmail":
          case "Identity.InvalidUserName":
          case "Identity.InvalidEmail":
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

      _logger.LogFunctionExit($"User Id: {result.Value.Id}");
      return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}


