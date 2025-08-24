using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Endpoints.IdentityRoles;
using ESLAdmin.Features.Identity.Endpoints.IdentityUsers.GetUser;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.GetUsers;

//------------------------------------------------------------------------------
//
//                        class GetUsersCommandHandler
//
//------------------------------------------------------------------------------
public class GetUsersCommandHandler :
  IdentityCommandHandlerBase<GetUsersCommandHandler>,
  ICommandHandler<
    GetUsersCommand,
    Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                        GetUsersCommandHandler
  //
  //------------------------------------------------------------------------------
  public GetUsersCommandHandler(
    IIdentityRepository repository,
    ILogger<GetUsersCommandHandler> logger) :
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>>
    ExecuteAsync(GetUsersCommand command, CancellationToken ct)
  {
    var usersResult = await _repository
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

    var userRolesResults = await _repository
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
