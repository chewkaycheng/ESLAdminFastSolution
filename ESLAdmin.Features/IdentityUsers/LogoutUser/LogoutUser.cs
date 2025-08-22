using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.IdentityUsers.LogoutUser;

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
    var accessToken = HttpContext.Request.Headers["Authorization"]
      .ToString().Replace("Bearer ", "");

    if (string.IsNullOrEmpty(userId) || 
        string.IsNullOrEmpty(accessToken))
    {
      return AppErrors
        .ProblemDetailsFactory
        .AuthenticationFailed();
    }

    return await new LogoutUserCommand
    {
      UserId = userId,
      Token = accessToken
    }.ExecuteAsync(ct);
  }
}
