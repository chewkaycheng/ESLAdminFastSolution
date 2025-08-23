using ESLAdmin.Features.Identity.Endpoints.IdentityRoles.GetRole;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityRoles.GetRoles;

//------------------------------------------------------------------------------
//
//                        class GetRolesCommand
//
//------------------------------------------------------------------------------
public class GetRolesCommand : ICommand<
  Results<Ok<IEnumerable<GetRoleResponse>>, ProblemDetails, InternalServerError>>
{
  public required GetRoleMapper Mapper { get; set; }
}
