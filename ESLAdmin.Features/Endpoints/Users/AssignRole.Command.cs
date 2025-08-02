using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;


namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class AssignRoleCommand
//
//-------------------------------------------------------------------------------
public class AssignRoleCommand : 
  ICommand<Results<NoContent, ProblemDetails, InternalServerError>>
{
  public string Email { get; set; } = string.Empty;
  public string RoleName { get; set; } = string.Empty;
}
