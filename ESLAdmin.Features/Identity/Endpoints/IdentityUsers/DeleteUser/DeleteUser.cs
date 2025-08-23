using ErrorOr;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.DeleteUser;

//-------------------------------------------------------------------------------
//
//                       class DeleteUserEndpoint
//
//-------------------------------------------------------------------------------
public class DeleteUserEndpoint : Endpoint<
  DeleteUserRequest,
  Results<Ok<Success>, ProblemDetails, InternalServerError>>
{
  private readonly ILogger<DeleteUserEndpoint> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       DeleteUserEndpoint
  //
  //-------------------------------------------------------------------------------
  public DeleteUserEndpoint(
    ILogger<DeleteUserEndpoint> logger)
  {
    _logger = logger;
  }

  //-------------------------------------------------------------------------------
  //
  //                       Configure
  //
  //-------------------------------------------------------------------------------
  public override void Configure()
  {
    Delete("/api/users/{email}");
    AllowAnonymous();
  }

  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public override async Task<Results<Ok<Success>, ProblemDetails, InternalServerError>> ExecuteAsync(
    DeleteUserRequest request,
    CancellationToken cancellationToken)
  {
    return await new DeleteUserCommand
    {
      Email = request.Email,
    }.ExecuteAsync();
  }
}
