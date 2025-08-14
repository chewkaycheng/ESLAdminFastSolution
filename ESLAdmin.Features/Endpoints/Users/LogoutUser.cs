using ESLAdmin.Common.Errors;
using ESLAdmin.Infrastructure.RepositoryManagers;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        LogoutUserEndpoint
//
//-------------------------------------------------------------------------------
public class LogoutUserEndpoint : EndpointWithoutRequest<
  Results<Ok, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;

  public LogoutUserEndpoint(
    IRepositoryManager repositoryManager)
  {
    _repositoryManager = repositoryManager;
  }

  public override void Configure()
  {
    Post("/api/auth/logout");
    AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
  }

  public override async Task<Results<Ok, ProblemDetails, InternalServerError>>
    ExecuteAsync(CancellationToken ct)
  {
    var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
    if (string.IsNullOrEmpty(userId))
    {
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(
          "InvalidUser",
          "User ID is missing or invalid."),
        StatusCodes.Status400BadRequest);
    }

    return await new LogoutUserCommand
    {
      UserId = userId
    }.ExecuteAsync(ct);
  }
}
