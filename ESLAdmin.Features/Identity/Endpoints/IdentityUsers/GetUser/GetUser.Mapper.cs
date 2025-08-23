using ESLAdmin.Domain.Dtos;
using ESLAdmin.Features.Identity.Entities;
using ESLAdmin.Infrastructure.Persistence.Entities;
using FastEndpoints;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.GetUser;

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
  public GetUserResponse EntityToResponse(User user, IList<string>? roleNames)
  {
    GetUserResponse response = new GetUserResponse()
    {
      Id = user.Id,
      FirstName = user.FirstName,
      LastName = user.LastName,
      UserName = user.UserName,
      Email = user.Email,
      PhoneNumber = user.PhoneNumber,
      Roles = roleNames
    };
    return response;
  }

  //------------------------------------------------------------------------------
  //
  //                          DtoToResponse
  //
  //-------------------------------------------------------------------------------
  public GetUserResponse DtoToResponse(UserDto userDto)
  {
    GetUserResponse response = new GetUserResponse()
    {
      Id = userDto.Id,
      FirstName = userDto.FirstName,
      LastName = userDto.LastName,
      UserName = userDto.UserName,
      Email = userDto.Email,
      PhoneNumber = userDto.PhoneNumber,
      Roles = userDto.Roles
    };
    return response;
  }
}
