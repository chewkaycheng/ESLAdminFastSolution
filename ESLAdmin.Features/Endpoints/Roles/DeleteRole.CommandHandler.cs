using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;

//-------------------------------------------------------------------------------
//
//                       class DeleteRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class DeleteRoleCommandHandler : ICommandHandler<
  DeleteRoleCommand,
   Results<
    NoContent,
    ProblemDetails,
    InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<DeleteRoleCommandHandler> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       DeleteRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public DeleteRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<DeleteRoleCommandHandler> logger)
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
      DeleteRoleCommand command,
      CancellationToken ct)
  {
    try
    {
      var result = await _repositoryManager.AuthenticationRepository.DeleteRoleAsync(command.Name);
      if (result.IsError)
      {
        foreach (var error in result.Errors)
        {
          if (error.Code == "Identity.RoleNotFound")
          {
            var validationFailures = new List<ValidationFailure>();
            validationFailures.AddRange(new ValidationFailure
            {
              PropertyName = error.Code,
              ErrorMessage = error.Description
            });
            return new ProblemDetails(validationFailures, StatusCodes.Status404NotFound);
          }

          return TypedResults.InternalServerError();
        }
      }

      return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }

  }
}
