using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Users.Models;
using FastEndpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Features.Endpoints.Users
{
  //------------------------------------------------------------------------------
  //
  //                          class GetUserMapper
  //
  //-------------------------------------------------------------------------------
  public class GetUserMapper : Mapper<GetUserRequest, UserResponse, User>
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
}
