using FastEndpoints;

namespace ESLAdmin.Features.Endpoints.Users;

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
  public LoginUserCommand ToCommand(LoginUserRequest loginRequest)
  {
    return new LoginUserCommand
    {
      Email = loginRequest.Email,
      Password = loginRequest.Password,
      RememberMe = loginRequest.RememberMe
    };
  }
}