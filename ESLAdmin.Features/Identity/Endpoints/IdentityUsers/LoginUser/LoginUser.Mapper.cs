using FastEndpoints;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.LoginUser;

//-------------------------------------------------------------------------------
//
//                       class LoginUserMapper
//
//-------------------------------------------------------------------------------
public class LoginUserMapper : Mapper<LoginUserRequest, LoginUserResponse, LoginUserCommand>
{
  //-------------------------------------------------------------------------------
  //
  //                       LoginUserCommand
  //
  //-------------------------------------------------------------------------------
  public LoginUserCommand RequestToCommand(LoginUserRequest loginRequest)
  {
    return new LoginUserCommand
    {
      Email = loginRequest.Email,
      Password = loginRequest.Password,
      RememberMe = loginRequest.RememberMe
    };
  }
}