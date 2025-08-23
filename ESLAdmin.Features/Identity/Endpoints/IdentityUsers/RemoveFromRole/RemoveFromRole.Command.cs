using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.RemoveFromRole;

//-------------------------------------------------------------------------------
//
//                       class RemoveFromRoleCommand
//
//-------------------------------------------------------------------------------
public class RemoveFromRoleCommand :
  ICommand<Results<NoContent, ProblemDetails, InternalServerError>>
{
  public string Email { get; set; } = string.Empty;
  public string RoleName { get; set; } = string.Empty;
}
