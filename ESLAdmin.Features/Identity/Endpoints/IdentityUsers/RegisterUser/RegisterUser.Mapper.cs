using ESLAdmin.Features.Identity.Entities;
using ESLAdmin.Infrastructure.Persistence.Entities;
using FastEndpoints;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.RegisterUser;

//------------------------------------------------------------------------------
//
//                      class RegisterUserMapper
//
//-------------------------------------------------------------------------------
public class RegisterUserMapper : Mapper<RegisterUserRequest, RegisterUserResponse, User>
{
  //------------------------------------------------------------------------------
  //
  //                          ToEntity
  //
  //-------------------------------------------------------------------------------
  public override User ToEntity(RegisterUserRequest request)
  {
    User user = new User()
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      UserName = request.UserName,
      Email = request.Email,
      PhoneNumber = request.PhoneNumber
    };
    return user;
  }

  //------------------------------------------------------------------------------
  //
  //                          CommandToEntity
  //
  //-------------------------------------------------------------------------------
  public User CommandToEntity(RegisterUserCommand command)
  {
    return new User
    {
      UserName = command.UserName,
      FirstName = command.FirstName,
      LastName = command.LastName,
      Email = command.Email,
      PhoneNumber = command.PhoneNumber,
    };
  }
}