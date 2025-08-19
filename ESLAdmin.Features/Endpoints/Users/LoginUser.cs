using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class LoginUserEndpoint
//
//-------------------------------------------------------------------------------
public class LoginUserEndpoint : Endpoint<LoginUserRequest,
  Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>,
  LoginUserMapper>
{
  //-------------------------------------------------------------------------------
  //
  //                       Configure
  //
  //-------------------------------------------------------------------------------
  public override void Configure()
  {
    Post("/api/auth/login");
    AllowAnonymous();
    Description(b => b
      .Produces<LoginUserResponse>(200)
      .ProducesProblem(400));
  }

  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public override async Task<Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(
    LoginUserRequest request,
    CancellationToken cancellationToken)
  {
    return await Map
      .RequestToCommand(request)
      .ExecuteAsync(cancellationToken);
  }
}