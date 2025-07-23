using ESLAdmin.Features.Users.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Users.Endpoints.GetUser
{
  //------------------------------------------------------------------------------
  //
  //                       class GetUserRequest
  //
  //-------------------------------------------------------------------------------
  public class GetUserRequest
  {
    public string Email  { get; set; }
  }

  public class GetUserCommand : ICommand<Results<Ok<UserResponse>, ProblemDetails, InternalServerError>>
  {
    public string Email { get; set; }
    public GetUserMapper Mapper { get; set; }
  }
}
