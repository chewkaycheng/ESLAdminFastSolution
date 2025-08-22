using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityUsers.RegisterUser;

//------------------------------------------------------------------------------
//
//                       class RegisterUserCommandHandler
//
//-------------------------------------------------------------------------------
public class RegisterUserCommandHandler : ICommandHandler<
      RegisterUserCommand,
      Results<Ok<Success>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<RegisterUserCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                       RegisterUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public RegisterUserCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<RegisterUserCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
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

      var result = await _repositoryManager
        .IdentityRepository
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


