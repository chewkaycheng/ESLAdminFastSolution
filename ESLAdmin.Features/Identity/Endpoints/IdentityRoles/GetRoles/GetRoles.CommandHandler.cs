using ESLAdmin.Features.Identity.Endpoints.IdentityRoles.GetRole;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityRoles.GetRoles;

//------------------------------------------------------------------------------
//
//                        class GetRolesCommandHandler
//
//------------------------------------------------------------------------------
public class GetRolesCommandHandler : IdentityCommandHandlerBase<GetRolesCommandHandler>, 
  ICommandHandler<
    GetRolesCommand,
      Results<
        Ok<IEnumerable<GetRoleResponse>>, 
        ProblemDetails, 
        InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                        GetRolesCommandHandler
  //
  //------------------------------------------------------------------------------
  public GetRolesCommandHandler(
    IIdentityRepository repository,
    ILogger<GetRolesCommandHandler> logger) :
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<
      Results<Ok<IEnumerable<GetRoleResponse>>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      GetRolesCommand command,
      CancellationToken ct)
  {
    var result = await _repository
      .GetAllRolesAsync();
    
    if (result.IsError)
    {
      var errorList = 
        result.Errors.Select(error => new ValidationFailure 
          { PropertyName = error.Code, 
            ErrorMessage = error.Description }).ToList();

      return new ProblemDetails(
        errorList,
        StatusCodes.Status500InternalServerError);
    }

    var roles = result.Value;

    IEnumerable<GetRoleResponse> response =
      roles.Select(role => command.Mapper.FromEntity(role)
      ).ToList();

    return TypedResults.Ok(response);
  }
}
