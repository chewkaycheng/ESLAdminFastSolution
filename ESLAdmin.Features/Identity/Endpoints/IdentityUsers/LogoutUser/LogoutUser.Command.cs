using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.LogoutUser;

//------------------------------------------------------------------------------
//
//                        class LogoutUserCommand
//
//-------------------------------------------------------------------------------
public class LogoutUserCommand : ICommand<Results<Ok, ProblemDetails, InternalServerError>>
{
  public string UserId { get; set; } = string.Empty;
  public string Token { get; set; } = string.Empty; // JWT token
}
