using ESLAdmin.Features.IdentityUsers.GetUser;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityUsers.GetUsers;

//------------------------------------------------------------------------------
//
//                        class GetUsers
//
//------------------------------------------------------------------------------
public class GetUsersEndpoint :
  EndpointWithoutRequest<Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>, GetUserMapper>
{
  private readonly ILogger<GetUsersEndpoint> _logger;
  //------------------------------------------------------------------------------
  //
  //                        GetUsersEndpoint
  //
  //------------------------------------------------------------------------------
  public GetUsersEndpoint(ILogger<GetUsersEndpoint> logger)
  {
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //------------------------------------------------------------------------------
  public override void Configure()
  {
    Get("api/users");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                        HandleAsync
  //
  //------------------------------------------------------------------------------
  public override async Task<Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>> ExecuteAsync(CancellationToken ct)
  {
    return await new GetUsersCommand
    {
      Mapper = new GetUserMapper()
    }.ExecuteAsync(ct); 
  }
}
