using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.IdentityRoles.UpdateRole;

//------------------------------------------------------------------------------
//
//                          class UpdateRoleCommand
//
//-------------------------------------------------------------------------------
public class UpdateRoleCommand : 
  ICommand<Results<Ok<string>, ProblemDetails, InternalServerError>>
{
  public required string OldName { get; set; }
  public required string NewName { get; set; }
}

