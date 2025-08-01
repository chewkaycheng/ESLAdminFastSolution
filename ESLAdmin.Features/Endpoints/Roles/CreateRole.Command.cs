using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Roles;

//-------------------------------------------------------------------------------
//
//                       class CreateRoleCommand
//
//-------------------------------------------------------------------------------
public class CreateRoleCommand : ICommand<
  Results<Ok<CreateRoleResponse>, 
    FastEndpoints.ProblemDetails, 
    InternalServerError>>
{
  public required string Name { get; set; }
  public required CreateRoleMapper Mapper { get; set; }
}
