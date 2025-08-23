using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Endpoints.IdentityRoles;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repository;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.RegisterUser;

//------------------------------------------------------------------------------
//
//                       class RegisterUserCommandHandler
//
//-------------------------------------------------------------------------------
public class RegisterUserCommandHandler : 
  IdentityCommandHandlerBase<RegisterUserCommandHandler>, 
  ICommandHandler<RegisterUserCommand,
      Results<Ok<Success>, 
        ProblemDetails, 
        InternalServerError>>
{ 
  //------------------------------------------------------------------------------
  //
  //                       RegisterUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public RegisterUserCommandHandler(
      IIdentityRepository repository,
      ILogger<RegisterUserCommandHandler> logger) :
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<Success>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      RegisterUserCommand command,
      CancellationToken cancellationToken)
  {
    try
    {
      var user = command.Mapper.CommandToEntity(command);

      var result = await _repository
        .RegisterUserAsync(
          user,
          command.Password,
          command.Roles);

      if (result.IsError)
      {
        var error = result.Errors.First();
        var statusCode = error.Code switch
        {
          "Identity.AddToRolesError" => StatusCodes.Status400BadRequest,
          "Identity.DuplicateUserName" => StatusCodes.Status400BadRequest,
          "Identity.DuplicateEmail" => StatusCodes.Status400BadRequest,
          "Identity.InvalidUserName" => StatusCodes.Status400BadRequest,
          "Identity.InvalidEmail" => StatusCodes.Status400BadRequest,
          "Identity.PasswordTooShort" => StatusCodes.Status400BadRequest,
          "Identity.PasswordRequiresNonAlphanumeric" => StatusCodes.Status400BadRequest,
          "Identity.PasswordRequiresDigit" => StatusCodes.Status400BadRequest,
          "Identity.PasswordRequiresLower" => StatusCodes.Status400BadRequest,
          "Identity.PasswordRequiresUpper" => StatusCodes.Status400BadRequest,
          "Identity.CreateUserFailed" => StatusCodes.Status400BadRequest,
          _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
          return TypedResults.InternalServerError();
        }
       
        return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            error.Code,
            error.Description),
          statusCode);
      }

      _logger.LogFunctionExit($"User Id: {user.Id}");
      return TypedResults.Ok(new Success());
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}


