using ESLAdmin.Features.Identity.Endpoints.IdentityUsers.GetUser;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.GetUsers;

//------------------------------------------------------------------------------
//
//                        class GetUsersCommand
//
//------------------------------------------------------------------------------
public class GetUsersCommand : ICommand<Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>>
{
  public GetUserMapper Mapper { get; set; }
}

