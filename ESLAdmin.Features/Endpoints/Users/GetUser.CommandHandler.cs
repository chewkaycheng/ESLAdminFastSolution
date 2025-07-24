using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

public class GetUserCommandHandler : ICommandHandler<
    GetUserCommand,
    Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetUserCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  public GetUserCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<GetUserCommandHandler> logger,
      IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _messageLogger = messageLogger;
  }

  public async Task<Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      GetUserCommand command,
      CancellationToken cancellationToken)
  {
    _logger.LogFunctionEntry($"Email: {command.Email}");
    try
    {
      var result = await _repositoryManager.AuthenticationRepository.GetUserByEmailAsync(
        command.Email);

      switch (result)
      {
        case null:
          _logger.LogNotFound("user", $"email: {command.Email}");

          var validationFailures = new List<ValidationFailure>();
          validationFailures.AddRange(new ValidationFailure
          {
            PropertyName = "NotFound",
            ErrorMessage = $"The user with email: {command.Email} is not found."
          });
          return new ProblemDetails(
            validationFailures,
            StatusCodes.Status404NotFound);
        default:
          var (user, roles) = result.Value;
          var userResponse = command.Mapper.ToResponse(user, roles?.ToList());

          DebugLogFunctionExit(user, roles);

          return TypedResults.Ok(userResponse);
      }
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      //_messageLogger.LogControllerException(
      //  nameof(ExecuteAsync),
      //  ex);

      return TypedResults.InternalServerError();
    }
  }

  private void DebugLogFunctionExit(User user, ICollection<string>? roles)
  {
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      var roleLog = roles != null ? string.Join(", ", roles) : "None";
      var context = $"\n=>User: \n    Username: '{user.UserName}', FirstName: '{user.FirstName}', LastName: '{user.LastName}', Email: '{user.Email}'\n    Password: '[Hidden]', PhoneNumber: '{user.PhoneNumber}', Roles: '{roleLog}'";
      _logger.LogFunctionEntry(context);
    }
  }

}
