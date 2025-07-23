using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Features.Users.Endpoints.RegisterUser;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Users.Endpoints.GetUser
{
  public class GetUserCommandHandler : ICommandHandler<
      GetUserCommand,
      Results<Ok<UserResponse>, ProblemDetails, InternalServerError>>
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

    public async Task<Results<Ok<UserResponse>, ProblemDetails, InternalServerError>>
      ExecuteAsync(
        GetUserCommand command,
        CancellationToken cancellationToken)
    {
      try
      {
        var userResponse = await _repositoryManager.AuthenticationRepository.GetUserByEmailAsync(
          command,
          command.Mapper);

        var apiResponse = new APIResponse<UserResponse>();

        if (userResponse == null)
        {
          var validationFailures = new List<ValidationFailure>();
          validationFailures.AddRange(new ValidationFailure
          {
            PropertyName = "NotFound",
            ErrorMessage = $"The user with email: {command.Email} is not found."
          });
          return new ProblemDetails(
            validationFailures,
            StatusCodes.Status404NotFound);
        }

        return TypedResults.Ok(userResponse);
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
