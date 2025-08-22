using ESLAdmin.Features.IdentityUsers.GetUser;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.IdentityUsers.GetUsers;

//------------------------------------------------------------------------------
//
//                        class GetUsersCommand
//
//------------------------------------------------------------------------------
public class GetUsersCommand : ICommand<Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>>
{
  public GetUserMapper Mapper { get; set; }
}

