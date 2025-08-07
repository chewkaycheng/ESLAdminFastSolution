using ESLAdmin.Common.Errors;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class AddToRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class AddToRoleCommandHandler : ICommandHandler<
  AddToRoleCommand,
  Results<NoContent, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<AddToRoleCommandHandler> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       AddToRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public AddToRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<AddToRoleCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------

  public async Task<Results<NoContent, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      AddToRoleCommand command,
      CancellationToken cancellationToken)
  {
    try
    {
      _logger.LogFunctionEntry($"Email: {command.Email}, RoleName: {command.RoleName}");

      var result = await _repositoryManager.AuthenticationRepository.AddToRoleAsync(
        command.Email,
        command.RoleName);

      if (result.IsError)
      {
        var error = result.Errors.First();
        var statusCode = StatusCodes.Status500InternalServerError;
        switch (error.Code)
        {
          case "User.UserNotFound":
          case "Role.NotFound":
            statusCode = StatusCodes.Status404NotFound;
            break;
          case "User.UserAlreadyInRole":
            statusCode = StatusCodes.Status400BadRequest;
            break;
          default:
            return TypedResults.InternalServerError();
        }
        return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            error.Code,
            error.Description), statusCode);
      }
      _logger.LogFunctionExit($"Email: {command.Email}, RoleName: {command.RoleName}");
      return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}

