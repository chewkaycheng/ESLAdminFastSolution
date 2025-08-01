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

      if (result.IsError)
      {
        foreach (var error in result.Errors)
        {
          if (error.Code == "Exception")
            return TypedResults.InternalServerError();

          var validateFailures = new List<ValidationFailure>();
          validateFailures.AddRange(new ValidationFailure
          {
            PropertyName = error.Code,
            ErrorMessage = error.Description
          });
          return new ProblemDetails(validateFailures, StatusCodes.Status401Unauthorized);
        }
      }
        
      var userLoginDto = result.Value;

      //Generate JWT token
      var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration.");
      var token = JwtBearer.CreateToken(
        o =>
        {
          o.SigningKey = jwtKey;
          o.ExpireAt = DateTime.UtcNow.AddDays(1);
          if (!string.IsNullOrEmpty(userLoginDto.UserName))
          {
            o.User.Claims.Add(("UserName", userLoginDto.UserName));
          }
          if (!string.IsNullOrEmpty(userLoginDto.Email))
          {
            o.User.Claims.Add(("Email", userLoginDto.Email));
          }

          o.User["UserId"] = userLoginDto.Id;
          if (userLoginDto.Roles != null && userLoginDto.Roles.Any())
          {
            o.User.Roles.Add(string.Join(",", userLoginDto.Roles));
          }
        });

      LoginUserResponse response = new LoginUserResponse
      {
        UserId = userLoginDto.Id,
        Email = userLoginDto.Email,
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