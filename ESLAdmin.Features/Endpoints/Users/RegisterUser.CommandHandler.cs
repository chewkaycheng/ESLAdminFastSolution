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
//                       class RegisterUserCommandHandler
//
//-------------------------------------------------------------------------------
public class RegisterUserCommandHandler : ICommandHandler<
      RegisterUserCommand,
      Results<NoContent, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<RegisterUserCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  public RegisterUserCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<RegisterUserCommandHandler> logger,
      IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _messageLogger = messageLogger;
  }

  public async Task<Results<NoContent, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      RegisterUserCommand command,
      CancellationToken cancellationToken)
  {
    try
    {
      var user = command.Mapper.CommandToEntity(command);

      var identityResultEx = await _repositoryManager.AuthenticationRepository.RegisterUserAsync(
        user,
        command.Password,
        command.Roles);

      if (!identityResultEx.Succeeded)
      {
        if (_logger.IsEnabled(LogLevel.Information))
        {
          _logger.LogValidationErrors(LoggingHelpers.FormatIdentityErrors(identityResultEx.Errors));
        }

        var validationFailures = identityResultEx.Errors.Select(error => new ValidationFailure
        {
          PropertyName = error.Code,
          ErrorMessage = error.Description
        }).ToList();

        return new ProblemDetails(validationFailures, StatusCodes.Status422UnprocessableEntity);
      }

      _logger.LogFunctionExit($"User Id: {identityResultEx.Id}");

      return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
      _messageLogger.LogControllerException(nameof(RegisterUserCommandHandler), ex);
      return TypedResults.InternalServerError();
    }
  }
}


