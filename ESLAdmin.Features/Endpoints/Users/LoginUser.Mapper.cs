using FastEndpoints;

namespace ESLAdmin.Features.Endpoints.Users;

public class LoginUserMapper : Mapper<LoginUserRequest, LoginUserResponse, LoginUserCommand>
{
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