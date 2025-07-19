using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Users.Models;
using FastEndpoints;

namespace ESLAdmin.Features.Users.Endpoints.GetUser;

//------------------------------------------------------------------------------
//
//                          class GetUserMapper
//
//-------------------------------------------------------------------------------
public class GetUserMapper : Mapper<GetUserRequest, UserResponse, User>
{
  //------------------------------------------------------------------------------
  //
  //                          FromEntity
  //
  //-------------------------------------------------------------------------------
  public override UserResponse FromEntity(User user)
  {
    UserResponse response = new UserResponse()
    {
      Id = user.Id,
      FirstName = user.FirstName,
      LastName = user.LastName,
      UserName = user.UserName,
      Email = user.Email,
      PhoneNumber = user.PhoneNumber
    };
    return response;
  }

  //------------------------------------------------------------------------------
  //
  //                          FromEntityWithRoles
  //
  //-------------------------------------------------------------------------------
  public UserResponse FromEntityWithRoles(User user, IList<string> roles)
  {
    UserResponse response = new UserResponse()
    {
      Id = user.Id,
      FirstName = user.FirstName,
      LastName = user.LastName,
      UserName = user.UserName,
      Email = user.Email,
      PhoneNumber = user.PhoneNumber,
      Roles = roles.ToList()
    };
    return response;
  }
}
