using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;

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
    try
    {
      var result = await _repositoryManager.IdentityRepository.GetAllRolesAsync();
      if (result.IsError)
      {
        foreach (var error in result.Errors)
        {
          if (error.Code == "Exception")
            return TypedResults.InternalServerError();
        }
      }

      var roles = result.Value;

      IEnumerable<GetRoleResponse> response =
        roles.Select(
          role => command.Mapper.FromEntity(role)
          ).ToList();

      return TypedResults.Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}
