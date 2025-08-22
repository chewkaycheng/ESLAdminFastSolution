using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.IdentityUsers.GetUser;

//------------------------------------------------------------------------------
//
//                       class GetUserCommand
//
//-------------------------------------------------------------------------------
public class GetUserCommand : ICommand<Results<Ok<GetUserResponse>, ProblemDetails, InternalServerError>>
{
  public string Email { get; set; } = string.Empty;
  public required GetUserMapper Mapper { get; set; }
}
