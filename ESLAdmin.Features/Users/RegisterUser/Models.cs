using FastEndpoints;

namespace ESLAdmin.Features.Users.RegisterUser;
public class Request
{
  public string UserId { get; set; }
}

public class Validator : Validator<Request>
{
  public Validator()
  {

  }
}