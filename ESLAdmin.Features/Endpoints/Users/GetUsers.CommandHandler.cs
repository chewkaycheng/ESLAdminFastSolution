using ESLAdmin.Domain.Dtos;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Endpoints.ChildcareLevels;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        class GetUsersCommandHandler
//
//------------------------------------------------------------------------------
public class GetUsersCommandHandler :
  ICommandHandler<
    GetUsersCommand,
    Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetUserCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                        GetUsersCommandHandler
  //
  //------------------------------------------------------------------------------
  public GetUsersCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<GetUserCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>> ExecuteAsync(GetUsersCommand command, CancellationToken ct)
  {
    var usersResult = await _repositoryManager.IdentityRepository.GetAllUsersAsync();
    if (usersResult.IsError)
    {
      return TypedResults.InternalServerError();
    }

    var users = usersResult.Value;

    var userRolesResults = await _repositoryManager.IdentityRepository.GetAllUserRolesAsync();
    if (userRolesResults.IsError)
    {
      return TypedResults.InternalServerError();
    }

    var userRoles = userRolesResults.Value;

    var usersResponse = new List<GetUserResponse>();
    foreach (var user in users)
    {
      var userResponse = new GetUserResponse
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        UserName = user.UserName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Roles = userRoles == null ? new List<string>() :
          userRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Name)
            .ToList()
      };
      usersResponse.Add(userResponse);
    }

    return TypedResults.Ok(usersResponse as IEnumerable<GetUserResponse>);
  }
}
