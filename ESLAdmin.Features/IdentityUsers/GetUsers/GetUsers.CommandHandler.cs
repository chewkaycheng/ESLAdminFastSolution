using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.IdentityUsers.GetUser;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityUsers.GetUsers;

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
  public async Task<Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>>
    ExecuteAsync(GetUsersCommand command, CancellationToken ct)
  {
    var usersResult = await _repositoryManager
      .IdentityRepository
      .GetAllUsersAsync();

    if (usersResult.IsError)
    {
      var error = usersResult.Errors.FirstOrDefault();
      var statusCode = error.Code switch
      {
        "Database.OperationCanceled" => StatusCodes.Status408RequestTimeout,
        _ => StatusCodes.Status500InternalServerError
      };

      return AppErrors
        .ProblemDetailsFactory
        .CreateProblemDetails(
          error.Code,
          error.Description,
          statusCode);
    }

    var users = usersResult.Value;

    var userRolesResults = await _repositoryManager
      .IdentityRepository
      .GetAllUserRolesAsync();

    if (userRolesResults.IsError)
    {
      var error = userRolesResults.Errors.FirstOrDefault();
      var statusCode = error.Code switch
      {
        "Database.OperationCanceled" => StatusCodes.Status408RequestTimeout,
        _ => StatusCodes.Status500InternalServerError
      };

      return AppErrors
       .ProblemDetailsFactory
       .CreateProblemDetails(
         error.Code,
         error.Description,
         StatusCodes.Status500InternalServerError);
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

    return TypedResults.Ok(
      usersResponse as IEnumerable<GetUserResponse>);
  }
}
