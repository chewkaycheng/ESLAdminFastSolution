using ESLAdmin.Common.Errors;
using ESLAdmin.Domain.Dtos;
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
        var error = result.Errors.First();
        return new ProblemDetails(
          ErrorUtils.CreateFailureList(error.Code, error.Description),
          StatusCodes.Status404NotFound );
      }

      var userDto = result.Value;
      var response = command.Mapper.DtoToResponse(result.Value);

      DebugLogFunctionExit(userDto);

      return TypedResults.Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }

  private void DebugLogFunctionExit(UserDto userDto)
  {
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      var roleLog = userDto.Roles != null ? string.Join(", ", userDto.Roles) : "None";
      var context = $"\n=>User: \n    Username: '{userDto.UserName}', FirstName: '{userDto.FirstName}', LastName: '{userDto.LastName}', Email: '{userDto.Email}'\n    Password: '[Hidden]', PhoneNumber: '{userDto.PhoneNumber}', Roles: '{roleLog}'";
      _logger.LogFunctionEntry(context);
    }
  }

}
