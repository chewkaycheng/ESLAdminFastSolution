using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

//-------------------------------------------------------------------------------
//
//                       class GetRoleCommand
//
//-------------------------------------------------------------------------------
namespace ESLAdmin.Features.Endpoints.Roles;

public class GetRoleCommand : ICommand<
  Results<Ok<GetRoleResponse>, 
  ProblemDetails, 
  InternalServerError>>
{
  public string Name { get; set; }
  public GetRoleMapper Mapper { get; set; }
}
