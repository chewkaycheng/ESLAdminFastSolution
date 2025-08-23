using ErrorOr;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.AddToRole;

//-------------------------------------------------------------------------------
//
//                       class AddToRoleCommand
//
//-------------------------------------------------------------------------------
public class AddToRoleCommand : 
  ICommand<Results<Ok<Success>, ProblemDetails, InternalServerError>>
{
  public string Email { get; set; } = string.Empty;
  public string RoleName { get; set; } = string.Empty;
}