using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.RefreshJwtToken;

//-------------------------------------------------------------------------------
//
//                       class RefreshTokenEndpoint
//
//-------------------------------------------------------------------------------
public class RefreshJwtTokenEndpoint : 
  Endpoint<RefreshJwtTokenRequest, 
    Results<Ok<RefreshJwtTokenResponse>, ProblemDetails, InternalServerError>>
{
  private readonly ILogger<RefreshJwtTokenEndpoint> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       RefreshTokenEndpoint
  //
  //-------------------------------------------------------------------------------
  public RefreshJwtTokenEndpoint(
    ILogger<RefreshJwtTokenEndpoint> logger)
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
    Post("/api/users/refresh-token");
    AllowAnonymous(); // No JWT required, as we're validating the refresh token
    Description(b => b
        .Produces<RefreshJwtTokenResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError));
  }

  //-------------------------------------------------------------------------------
  //
  //                       Configure
  //
  //-------------------------------------------------------------------------------
  public override async Task<Results<Ok<RefreshJwtTokenResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      RefreshJwtTokenRequest req, 
      CancellationToken ct)
  {
    try
    {
      var command = new RefreshJwtTokenCommand
      {
        AccessToken = req.AccessToken,
        RefreshToken = req.RefreshToken
      };
      return await command.ExecuteAsync(ct);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}
