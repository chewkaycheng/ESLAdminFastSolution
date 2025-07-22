using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ESLAdmin.Features.Users.Endpoints.RegisterUser;

//------------------------------------------------------------------------------
//
//                       class RegisterUserEndpoint
//
//-------------------------------------------------------------------------------
public class RegisterUserEndpoint : Endpoint<
  RegisterUserRequest,
  Results<NoContent,
    ProblemDetails,
    InternalServerError>,
  RegisterUserMapper>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly IMessageLogger _messageLogger;
  private readonly ILogger<RegisterUserEndpoint> _logger;

  //------------------------------------------------------------------------------
  //
  //                       RegisterUserEndpoint
  //
  //-------------------------------------------------------------------------------
  public RegisterUserEndpoint(
    IRepositoryManager repositoryManager,
    ILogger<RegisterUserEndpoint> logger,
    IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _messageLogger = messageLogger;
  }

  //------------------------------------------------------------------------------
  //
  //                       Configure
  //
  //-------------------------------------------------------------------------------
  public override void Configure()
  {
    Post("/api/register");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public override async
    Task<Results<NoContent,
      ProblemDetails,
      InternalServerError>>
    ExecuteAsync(
      RegisterUserRequest request,
      CancellationToken cancellationToken)
  {
    var Roles = request.Roles != null ? string.Join(", ", request.Roles) : "None";
    
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      var roleLog = Roles = request.Roles != null ? string.Join(", ", request.Roles) : "None";
      var context = $"\n=>Request: \n    Username: '{request.UserName}', FirstName: '{request.FirstName}', LastName: '{request.LastName}', Email: '{request.Email}'\n    Password: '[Hidden]', PhoneNumber: '{request.PhoneNumber}', Roles: '{roleLog}'";
      _logger.LogFunctionEntry(context);
    }

    try
    {
      var identityResultEx = await _repositoryManager.AuthenticationRepository.RegisterUserAsync(
      request, Map);

      if (!identityResultEx.Succeeded)
      {
        if (_logger.IsEnabled(LogLevel.Information))
        {
          _logger.LogValidationErrors(
            LoggingHelpers.FormatIdentityErrors(
              identityResultEx.Errors));
        }

        ValidationFailures.AddRange(
          identityResultEx.Errors.Select(error => new ValidationFailure
          {
            PropertyName = error.Code,
            ErrorMessage = error.Description
          }));

        return new ProblemDetails(
          ValidationFailures, 
          StatusCodes.Status422UnprocessableEntity);
      }

      _logger.LogFunctionExit($"User Id: {identityResultEx.Id}");

      HttpContext.Response.Headers.Append(
        "location", $"/api/users/{request.Email}");

      return TypedResults.NoContent();
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