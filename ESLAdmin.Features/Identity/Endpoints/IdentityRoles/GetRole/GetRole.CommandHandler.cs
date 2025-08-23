using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repository;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityRoles.GetRole;

//-------------------------------------------------------------------------------
//
//                       class GetRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class GetRoleCommandHandler :
  IdentityCommandHandlerBase<GetRoleCommandHandler>,
    ICommandHandler<
      GetRoleCommand,
        Results<
          Ok<GetRoleResponse>,
          ProblemDetails,
          InternalServerError>>
{
  //-------------------------------------------------------------------------------
  //
  //                       GetRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public GetRoleCommandHandler(
    IIdentityRepository repository,
    ILogger<GetRoleCommandHandler> logger) :
    base(repository, logger)
  {
  }
  
  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<GetRoleResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      GetRoleCommand command,
      CancellationToken ct)
  {
    var result = await _repository
      .GetRoleAsync(command.Name);

    if (result.IsError)
    {
      _logger.LogNotFound("role", $"name: '{command.Name}'");
      var error = result.Errors.First();
      if (error.Code == "Identity.RoleNotFound")
      {
        AppErrors
          .ProblemDetailsFactory
          .CreateProblemDetails(
            error.Code,
            error.Description,
            StatusCodes.Status404NotFound);
      }

      return TypedResults.InternalServerError();
    }

    var response = command.Mapper.FromEntity(result.Value);
    return TypedResults.Ok(response);
  }
}