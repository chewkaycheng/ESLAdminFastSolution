using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Features.Endpoints.Roles;

//-------------------------------------------------------------------------------
//
//                       class DeleteRoleCommand
//
//-------------------------------------------------------------------------------
public class GetRoleMapper : Mapper<GetRoleRequest, GetRoleResponse, IdentityRole>
{
  //------------------------------------------------------------------------------
  //
  //                          FromEntity
  //
  //-------------------------------------------------------------------------------
  public override GetRoleResponse FromEntity(IdentityRole role) =>
    new GetRoleResponse
    {
      Id = role.Id,
      Name = role.Name
    };
}
