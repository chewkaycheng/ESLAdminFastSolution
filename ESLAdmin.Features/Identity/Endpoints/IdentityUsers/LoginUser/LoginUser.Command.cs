using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.LoginUser;

//-------------------------------------------------------------------------------
//
//                       LoginUserCommand
//
//-------------------------------------------------------------------------------
public class LoginUserCommand :
    ICommand<Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool RememberMe { get; set; }
}