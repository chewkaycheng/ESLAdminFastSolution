using ESLAdmin.Domain.Dtos;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        class GetUsersCommand
//
//------------------------------------------------------------------------------
public class GetUsersCommand : ICommand<Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>>
{
  public GetUserMapper Mapper { get; set; }
}

