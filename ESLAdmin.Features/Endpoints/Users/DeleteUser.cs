using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users
{
  public class DeleteUserEndpoint : Endpoint<
    DeleteUserRequest,
    Results<Ok<string>, ProblemDetails, InternalServerError>>
  {
    private readonly IRepositoryManager _repositoryManager;
    private readonly IMessageLogger _messageLogger;

    //-------------------------------------------------------------------------------
    //
    //                       DeleteUserEndpoint
    //
    //-------------------------------------------------------------------------------
    public DeleteUserEndpoint(
      IRepositoryManager repositoryManager,
      IMessageLogger messageLogger)
    {
      _repositoryManager = repositoryManager;
      _messageLogger = messageLogger;
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
    public override async Task<Results<Ok<string>, ProblemDetails, InternalServerError>> ExecuteAsync(
      DeleteUserRequest request,
      CancellationToken cancellationToken)
    {
      return await new DeleteUserCommand
      {
        Email = request.Email,
      }.ExecuteAsync();
    }
  }
}
