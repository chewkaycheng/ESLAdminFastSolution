using ESLAdmin.Common.Errors;
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
//                       class RegisterUserCommandHandler
//
//-------------------------------------------------------------------------------
public class RegisterUserCommandHandler : ICommandHandler<
      RegisterUserCommand,
      Results<NoContent, ProblemDetails, InternalServerError>>
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
  public async Task<Results<NoContent, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      RegisterUserCommand command,
      CancellationToken cancellationToken)
  {
    try
    {
      var user = command.Mapper.CommandToEntity(command);

      var result = await _repositoryManager.AuthenticationRepository.RegisterUserAsync(
        user,
        command.Password,
        command.Roles);

      if (result.IsError)
      {
        var error = result.Errors.First();
        if (error.Code == "Identity.CreateUserFailed" ||
            error.Code == "Identity.AddToRolesFailed" ||
            error.Code == "Exception")
          return TypedResults.InternalServerError();

        return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            error.Code,
            error.Description),
          StatusCodes.Status400BadRequest);
      }

      _logger.LogFunctionExit($"User Id: {result.Value.Id}");
      return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}


