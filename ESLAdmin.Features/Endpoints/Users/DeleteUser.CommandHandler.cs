using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Infrastructure.RepositoryManagers;
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
//                        class DeleteUserCommandHandler
//
//-------------------------------------------------------------------------------
public class DeleteUserCommandHandler : ICommandHandler<
  DeleteUserCommand,
  Results<Ok<string>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetUserCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                        DeleteUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public DeleteUserCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<GetUserCommandHandler> logger,
      IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _messageLogger = messageLogger;
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<string>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      DeleteUserCommand command,
      CancellationToken cancellationToken)
  {
    _logger.LogFunctionEntry($"Email: {command.Email}");
    if (string.IsNullOrEmpty(command.Email))
    {
      var validationFailures = new List<ValidationFailure>();
      validationFailures.AddRange(new ValidationFailure()
      {
        PropertyName = "NullOrEmpty",
        ErrorMessage = "The email cannot be null or empty"
      });
      return new ProblemDetails(validationFailures, StatusCodes.Status400BadRequest);
    }

    try
    {
      var result = await _repositoryManager.AuthenticationRepository.DeleteUserByEmailAsync(
        command.Email);

      if (result.Succeeded)
      {
        return TypedResults.Ok(result.Id);
      }

      var validationFailures = new List<ValidationFailure>();
      if (result.Errors.Count() == 1)
      {
        if (result.Errors.FirstOrDefault()?.Code == "NotFound")
        {
          validationFailures.AddRange(new ValidationFailure
          {
            PropertyName = "NotFound",
            ErrorMessage = $"The user with email: {command.Email} is not found."
          });
          return new ProblemDetails(
            validationFailures,
            StatusCodes.Status404NotFound);
        }
        else if (result.Errors.FirstOrDefault()?.Code == "Exception")
        {
          return TypedResults.InternalServerError();
        }
      }

      if (result.ErrorType == IdentityErrorTypes.DeleteUserError)
      {
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "DeleteUserError",
          ErrorMessage = "An error occurred while deleting user"
        });
        return new ProblemDetails(
          validationFailures, StatusCodes.Status400BadRequest);
      }
      else
      {
        // Remove roles error
        return TypedResults.InternalServerError();
      }
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}
