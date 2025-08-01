using ESLAdmin.Infrastructure.RepositoryManagers;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;

//------------------------------------------------------------------------------
//
//                          class CreateRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class CreateRoleCommandHandler : ICommandHandler<
  CreateRoleCommand,
  Results<Ok<CreateRoleResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<CreateRoleCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                          CreateRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public CreateRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<CreateRoleCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                          ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<CreateRoleResponse>, ProblemDetails, InternalServerError>> 
    ExecuteAsync(
      CreateRoleCommand command, 
      CancellationToken ct)
  {
    var result = await _repositoryManager.AuthenticationRepository.CreateRoleAsync(command.Name);
    if (result.IsError)
    {
      foreach(var error in result.Errors)
      {
        if ((error.Code == "Exception") || (error.Code == "Role.CreateFailed"))
        {
          return TypedResults.InternalServerError();
        }
        
        var validationFailures = new List<ValidationFailure>();
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = error.Code,
          ErrorMessage = error.Description
        });
        return new ProblemDetails(validationFailures, StatusCodes.Status409Conflict);
      }
    }

    return TypedResults.Ok(command.Mapper.FromEntity(result.Value));
  }
}
