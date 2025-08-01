using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;

//------------------------------------------------------------------------------
//
//                       class UpdateRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class UpdateRoleCommandHandler : ICommandHandler<
  UpdateRoleCommand,
  Results<Ok<string>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<UpdateRoleCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                       UpdateRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public UpdateRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<UpdateRoleCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<string>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      UpdateRoleCommand command,
      CancellationToken ct)
  {
    try
    {
      var result = await _repositoryManager.AuthenticationRepository.UpdateRoleAsync(command.OldName, command.NewName);

      if (result.IsError)
      {
        foreach (var error in result.Errors)
        {
          if (error.Code == "Role.UpdateFailed" || error.Code == "Exception")
          {
            return TypedResults.InternalServerError();
          }

          var validationFailures = new List<ValidationFailure>();
          var statusCode = error.Code switch
          {
            "Role.NotFound" => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status409Conflict
          };
          validationFailures.AddRange(new ValidationFailure
          {
            PropertyName = error.Code,
            ErrorMessage = error.Description
          });
          return new ProblemDetails(validationFailures, statusCode);
        }
      }

      return TypedResults.Ok(result.Value);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}
