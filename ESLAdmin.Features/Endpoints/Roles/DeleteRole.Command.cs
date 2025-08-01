using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Roles;

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

