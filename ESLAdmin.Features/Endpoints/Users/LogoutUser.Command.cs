using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Windows.Input;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        class LogoutUserCommand
//
//-------------------------------------------------------------------------------
public class LogoutUserCommand : ICommand<Results<Ok, ProblemDetails, InternalServerError>>
{
  public string UserId { get; set; } = string.Empty;
}
