using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Domain.Dtos;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
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
      var userResult = await _repositoryManager
        .IdentityRepository
        .GetUserByEmailAsync(
          command.Email);
      
      if (userResult.IsError)
      {
        var error = userResult.Errors.First();
        var statusCode = error.Code switch
        {
          "Database.ConcurrencyFailure" => StatusCodes.Status409Conflict,
          "Database.OperationCanceled" => StatusCodes.Status408RequestTimeout,
          string code when code.Contains("Exception") => StatusCodes.Status500InternalServerError,
          _ => StatusCodes.Status404NotFound
        };

        return AppErrors
          .ProblemDetailsFactory
          .CreateProblemDetails(
            error.Code,
            error.Description,
            statusCode);
      }

      var user = userResult.Value;

      var roleResult = await _repositoryManager.IdentityRepository.GetRolesForUserAsync(user);

      if (roleResult.IsError)
      {
        var error = roleResult.Errors.First();
        var statusCode = error.Code switch
        {
          "Database.ConcurrencyFailure" => StatusCodes.Status409Conflict,
          "Database.OperationCanceled" => StatusCodes.Status408RequestTimeout,
          string code when code.Contains("Exception") => StatusCodes.Status500InternalServerError,
          _ => StatusCodes.Status404NotFound
        };

        return AppErrors
         .ProblemDetailsFactory
         .CreateProblemDetails(
           error.Code,
           error.Description,
           statusCode);
      }

      var roleNames = roleResult.Value;
      var response = command.Mapper.EntityToResponse(user, roleNames);

      DebugLogFunctionExit(response);
      return TypedResults.Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }

  private void DebugLogFunctionExit(GetUserResponse userDto)
  {
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      var roleLog = userDto.Roles != null ? string.Join(", ", userDto.Roles) : "None";
      var context = $"\n=>User: \n    Username: '{userDto.UserName}', FirstName: '{userDto.FirstName}', LastName: '{userDto.LastName}', Email: '{userDto.Email}'\n    Password: '[Hidden]', PhoneNumber: '{userDto.PhoneNumber}', Roles: '{roleLog}'";
      _logger.LogFunctionEntry(context);
    }
  }

}
