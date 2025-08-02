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


//------------------------------------------------------------------------------
//
//                       class GetUserCommandHandler
//
//-------------------------------------------------------------------------------
public class GetUserCommandHandler : ICommandHandler<
    GetUserCommand,
    Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetUserCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                       GetUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public GetUserCommandHandler(
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
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
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

      if (result.IsError)
      {
        foreach (var error in result.Errors)
        {
          if (error.Code == "Exception")
          {
            return TypedResults.InternalServerError();
          }

          var validationFailures = new List<ValidationFailure>();
          validationFailures.AddRange(new ValidationFailure
          {
            PropertyName = error.Code,
            ErrorMessage = error.Description
          });
          return new ProblemDetails(validationFailures, StatusCodes.Status404NotFound);
        }
      }

      var response = command.Mapper.DtoToResponse(result.Value);
      return TypedResults.Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

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
