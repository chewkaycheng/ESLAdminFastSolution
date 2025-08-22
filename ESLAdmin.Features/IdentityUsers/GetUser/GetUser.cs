using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityUsers.GetUser;

//-------------------------------------------------------------------------------
//
//                          class GetUserEndpoint
//
//-------------------------------------------------------------------------------
public class GetUserEndpoint : Endpoint<
  GetUserRequest,
  Results<Ok<GetUserResponse>,
    ProblemDetails,
    InternalServerError>,
  GetUserMapper>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetUserEndpoint> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       GetUserEndpoint
  //
  //-------------------------------------------------------------------------------
  public GetUserEndpoint(
    IRepositoryManager repositoryManager,
    ILogger<GetUserEndpoint> logger)
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
    Get("/api/users/{email}");
    AllowAnonymous();
  }

  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public override async Task<Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(
    GetUserRequest request,
    CancellationToken cancellationToken)
  {
    return await new GetUserCommand
    {
      Email = request.Email,
      Mapper = Map
    }.ExecuteAsync();
  }
}
