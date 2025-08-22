using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.IdentityRoles.DeleteRole;

//-------------------------------------------------------------------------------
//
//                       class DeleteRoleCommand
//
//-------------------------------------------------------------------------------
public class DeleteRoleCommand : ICommand<
  Results<
    NoContent,
    ProblemDetails,
    InternalServerError>>
{
  public required string Name { get; set; }
}

