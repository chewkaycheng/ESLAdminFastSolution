using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users;

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
  private readonly IMessageLogger _messageLogger;

  //-------------------------------------------------------------------------------
  //
  //                       GetUserEndpoint
  //
  //-------------------------------------------------------------------------------
  public GetUserEndpoint(
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
