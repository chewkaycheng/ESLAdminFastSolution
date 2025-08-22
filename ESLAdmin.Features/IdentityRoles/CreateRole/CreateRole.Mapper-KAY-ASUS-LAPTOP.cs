using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Features.IdentityRoles.CreateRole;

//------------------------------------------------------------------------------
//
//                          class CreateRoleMapper
//
//-------------------------------------------------------------------------------
public class CreateRoleMapper : Mapper<CreateRoleRequest, CreateRoleResponse, IdentityRole>
{
  public CreateRoleCommand ToCommand(
    CreateRoleRequest request,
    CreateRoleMapper mapper)
  {
    return new CreateRoleCommand
    {
      Name = request.Name,
      Mapper = mapper
    };
  }

  //------------------------------------------------------------------------------
  //
  //                          FromEntity
  //
  //-------------------------------------------------------------------------------
  public override CreateRoleResponse FromEntity(IdentityRole role) =>
    new CreateRoleResponse
    {
      Id = role.Id,
      Name = role.Name
    };
}
