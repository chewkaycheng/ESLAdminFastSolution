using ESLAdmin.Features.Users.Models;
using ESLAdmin.Infrastructure.RepositoryManagers;
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
    try
    {
      var result = await _repositoryManager.AuthenticationRepository.GetUserByEmailAsync(
        command.Email);

      switch (result)
      {
        case null:
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
          return TypedResults.Ok(userResponse);
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogControllerException(
        nameof(ExecuteAsync),
        ex);

      return TypedResults.InternalServerError();
    }
  }
}
