using ESLAdmin.Features.IdentityRoles.GetRole;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityRoles.GetRoles;

//------------------------------------------------------------------------------
//
//                        class GetRolesCommandHandler
//
//------------------------------------------------------------------------------
public class GetRolesCommandHandler : ICommandHandler<
  GetRolesCommand,
  Results<Ok<IEnumerable<GetRoleResponse>>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetRolesCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                        GetRolesCommandHandler
  //
  //------------------------------------------------------------------------------
  public GetRolesCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<GetRolesCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
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
    var result = await _repositoryManager
      .IdentityRepository
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
