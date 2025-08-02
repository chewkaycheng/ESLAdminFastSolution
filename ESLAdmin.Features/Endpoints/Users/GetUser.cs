using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

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
    try
    {
      return await new GetUserCommand
      {
        Email = request.Email,
        Mapper = Map
      }.ExecuteAsync();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}
