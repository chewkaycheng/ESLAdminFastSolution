using Dapper;
using ESLAdmin.Domain.Entities;
using FastEndpoints;

namespace ESLAdmin.Features.Users.RegisterUser;
public class Mapper : Mapper<Request, Response, User>
{
  public override User ToEntity(Request r)
  {
    User user = new User()
    {
      FirstName = r.FirstName,
      LastName = r.LastName,
      UserName = r.UserName,
      Email = r.Email,
      PhoneNumber = r.PhoneNumber
    };
    return user;
  }

  public override Response FromEntity(User e)
  {
    return base.FromEntity(e);
  }
}