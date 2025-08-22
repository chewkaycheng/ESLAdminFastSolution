namespace ESLAdmin.Features.IdentityRoles.UpdateRole;

//------------------------------------------------------------------------------
//
//                          class UpdateRoleRequest
//
//-------------------------------------------------------------------------------
public class UpdateRoleRequest
{
  public required string OldName { get; set; }
  public required string NewName { get; set; }
}
