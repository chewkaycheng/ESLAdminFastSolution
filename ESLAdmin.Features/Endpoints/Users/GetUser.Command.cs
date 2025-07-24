using ESLAdmin.Features.Users.Endpoints.GetUser;
using ESLAdmin.Features.Users.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                       class GetUserCommand
//
//-------------------------------------------------------------------------------
public class GetUserCommand : ICommand<Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>>
{
  public string Email { get; set; }
  public GetUserMapper Mapper { get; set; }
}
