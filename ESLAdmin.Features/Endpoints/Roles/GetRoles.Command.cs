using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Roles;

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
