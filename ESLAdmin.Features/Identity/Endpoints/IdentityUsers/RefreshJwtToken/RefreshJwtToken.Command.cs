using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.RefreshJwtToken;

//-------------------------------------------------------------------------------
//
//                        class RefreshTokenCommand
//
//-------------------------------------------------------------------------------
public class RefreshJwtTokenCommand : 
  ICommand<Results<Ok<RefreshJwtTokenResponse>, ProblemDetails, InternalServerError>>
{
  public string AccessToken { get; set; } 
  public string RefreshToken { get; set; }
}
