using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Endpoints.IdentityRoles;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.GetUser;


//------------------------------------------------------------------------------
//
//                       class GetUserCommandHandler
//
//-------------------------------------------------------------------------------
public class GetUserCommandHandler : IdentityCommandHandlerBase<GetUserCommandHandler>,
  ICommandHandler<GetUserCommand,
    Results<
      Ok<GetUserResponse>, 
      ProblemDetails, 
      InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                       GetUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public GetUserCommandHandler(
      IIdentityRepository repository,
      ILogger<GetUserCommandHandler> logger) :
    base(repository, logger)
  {
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
      var userResult = await _repository
        .GetUserByEmailAsync(
          command.Email);
      
      if (userResult.IsError)
      {
        var error = userResult.Errors.First();
        var statusCode = error.Code switch
        {
          "Identity.Exception.InvalidArgument" => StatusCodes.Status400BadRequest,
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

      var roleResult = await _repository
        .GetRolesForUserAsync(user);

      if (roleResult.IsError)
      {
        var error = roleResult.Errors.First();
        var statusCode = error.Code switch
        {
          "Identity.Exception.InvalidArgument" => StatusCodes.Status400BadRequest,
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
      _logger.LogFunctionExit(context);
    }
  }

}
