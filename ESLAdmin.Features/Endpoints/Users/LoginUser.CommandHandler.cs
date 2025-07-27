using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand,
    Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<LoginUserCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  public LoginUserCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<LoginUserCommandHandler> logger,
      IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _messageLogger = messageLogger;
  }

  public async Task<Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
      ExecuteAsync(
          LoginUserCommand command,
          CancellationToken cancellationToken)
  {
    try
    {
      var result = await _repositoryManager.AuthenticationRepository.Login(
          command.Email, command.Password);

      if (result == null)
      {
        var validationFailures = new List<ValidationFailure>();
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "LoginFailure",
          ErrorMessage = "Invalid email or password."
        });
        return new ProblemDetails(validationFailures, StatusCodes.Status400BadRequest);
      }

      var (user, roles) = result.Value;

      //Generate JWT token
      var token = JwtBearer.CreateToken(
        o =>
        {
          o.SigningKey = "RJRKRKJRJRKRJJKR";
          o.ExpireAt = DateTime.UtcNow.AddDays(1);
          o.User.Claims.Add(("UserName", user.UserName));
          o.User["UserId"] = user.Id;
          if (roles != null && roles.Any())
          {
            foreach (var role in roles)
            {
              o.User.Roles.Add(role);
            }
            ;
          }
          ;
        });

      LoginUserResponse response = new LoginUserResponse
      {
        UserId = user.Id,
        Email = user.Email,
        Token = token
      };

      return TypedResults.Ok(response);

    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}