using ErrorOr;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class DeleteUserEndpoint
//
//-------------------------------------------------------------------------------
public class DeleteUserEndpoint : Endpoint<
  DeleteUserRequest,
  Results<Ok<Success>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<DeleteUserEndpoint> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       DeleteUserEndpoint
  //
  //-------------------------------------------------------------------------------
  public DeleteUserEndpoint(
    IRepositoryManager repositoryManager,
    ILogger<DeleteUserEndpoint> logger)
  {
    _repositoryManager = repositoryManager;
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
