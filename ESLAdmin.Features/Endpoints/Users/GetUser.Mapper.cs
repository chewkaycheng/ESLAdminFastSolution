using ESLAdmin.Domain.Entities;
using FastEndpoints;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                          class GetUserMapper
//
//-------------------------------------------------------------------------------
public class GetUserMapper : Mapper<GetUserRequest, GetUserResponse, User>
{
  //------------------------------------------------------------------------------
  //
  //                          ToResponse
  //
  //-------------------------------------------------------------------------------
  public GetUserResponse ToResponse(User user, IList<string>? roles)
  {
    GetUserResponse response = new GetUserResponse()
    {
      Id = user.Id,
      FirstName = user.FirstName,
      LastName = user.LastName,
      UserName = user.UserName,
      Email = user.Email,
      PhoneNumber = user.PhoneNumber,
      Roles = roles
    };
    return response;
  }
}
