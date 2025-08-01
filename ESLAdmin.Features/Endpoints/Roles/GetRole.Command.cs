using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Roles;

//-------------------------------------------------------------------------------
//
//                       class GetRoleCommand
//
//-------------------------------------------------------------------------------
public class GetRoleCommand : ICommand<
  Results<Ok<GetRoleResponse>,
  ProblemDetails,
  InternalServerError>>
{
  public string Name { get; set; }
  public GetRoleMapper Mapper { get; set; }
}
