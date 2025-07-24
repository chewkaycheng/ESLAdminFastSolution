using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Users.Models;
using FastEndpoints;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                      class RegisterUserMapper
//
//-------------------------------------------------------------------------------
public class RegisterUserMapper : Mapper<RegisterUserRequest, UserResponse, User>
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