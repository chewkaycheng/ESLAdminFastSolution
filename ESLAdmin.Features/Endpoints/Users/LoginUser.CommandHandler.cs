using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class LoginUserCommandHandler
//
//-------------------------------------------------------------------------------
public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand,
    Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<LoginUserCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;
  private readonly IConfiguration _config;

  //-------------------------------------------------------------------------------
  //
  //                       LoginUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public LoginUserCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<LoginUserCommandHandler> logger,
      IMessageLogger messageLogger,
      IConfiguration config)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _messageLogger = messageLogger;
    _config = config;
  }

  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
      ExecuteAsync(
          LoginUserCommand command,
          CancellationToken cancellationToken)
  {
    try
    {
      var result = await _repositoryManager.AuthenticationRepository.LoginAsync(
          command.Email, command.Password);

      if (result == null)
      {
        var validationFailures = new List<ValidationFailure>();
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "LoginFailure",
          ErrorMessage = "Your email or password is not valid."
        });
        return new ProblemDetails(validationFailures, StatusCodes.Status400BadRequest);
      }

      var (user, roles) = result.Value;

      //Generate JWT token
      var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration.");
      var token = JwtBearer.CreateToken(
        o =>
        {
          o.SigningKey = jwtKey;
          o.ExpireAt = DateTime.UtcNow.AddDays(1);
          if (!string.IsNullOrEmpty(user.UserName))
          {
            o.User.Claims.Add(("UserName", user.UserName));
          }
          o.User["UserId"] = user.Id;
          if (roles != null && roles.Any())
          {
            o.User.Roles.Add(string.Join(",", roles));
          }
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