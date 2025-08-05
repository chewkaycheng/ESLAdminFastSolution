using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                        class RefreshTokenCommand
//
//-------------------------------------------------------------------------------
public class RefreshTokenCommand : 
  ICommand<Results<Ok<RefreshTokenResponse>, ProblemDetails, InternalServerError>>
{
  public string AccessToken { get; set; } 
  public string RefreshToken { get; set; }
}
