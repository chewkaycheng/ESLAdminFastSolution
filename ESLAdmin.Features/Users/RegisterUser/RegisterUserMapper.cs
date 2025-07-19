using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Users.Models;
using FastEndpoints;

namespace ESLAdmin.Features.Users.RegisterUser;

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
}