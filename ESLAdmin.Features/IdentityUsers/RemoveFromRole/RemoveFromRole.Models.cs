namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        class RemoveFromRoleRequest
//
//-------------------------------------------------------------------------------
public class RemoveFromRoleRequest
{
  public string Email { get; set; } = string.Empty;
  public string RoleName { get; set; } = string.Empty;
}
