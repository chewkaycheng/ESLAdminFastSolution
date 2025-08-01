using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics.CodeAnalysis;

namespace ESLAdmin.Features.Endpoints.Roles;

//------------------------------------------------------------------------------
//
//                          class UpdateRoleCommand
//
//-------------------------------------------------------------------------------
public class UpdateRoleCommand : ICommand<Results<Ok<string>, ProblemDetails, InternalServerError>>
{
  public required string OldName { get; set; }
  public required string NewName { get; set; }
}

