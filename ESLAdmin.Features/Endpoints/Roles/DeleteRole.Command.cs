using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

//-------------------------------------------------------------------------------
//
//                       class DeleteRoleCommand
//
//-------------------------------------------------------------------------------
namespace ESLAdmin.Features.Endpoints.Roles;

public class DeleteRoleCommand : ICommand<
  Results<
    NoContent, 
    ProblemDetails, 
    InternalServerError>>
{
  public required string Name { get; set; }
}

