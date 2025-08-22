using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class RefreshTokenEndpoint
//
//-------------------------------------------------------------------------------
public class RefreshTokenEndpoint : 
  Endpoint<RefreshTokenRequest, 
    Results<Ok<RefreshTokenResponse>, ProblemDetails, InternalServerError>>
{
  private readonly ILogger<RefreshTokenEndpoint> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       RefreshTokenEndpoint
  //
  //-------------------------------------------------------------------------------
  public RefreshTokenEndpoint(
    ILogger<RefreshTokenEndpoint> logger)
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
        .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError));
  }

  //-------------------------------------------------------------------------------
  //
  //                       Configure
  //
  //-------------------------------------------------------------------------------
  public override async Task<Results<Ok<RefreshTokenResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      RefreshTokenRequest req, 
      CancellationToken ct)
  {
    try
    {
      var command = new RefreshTokenCommand
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
