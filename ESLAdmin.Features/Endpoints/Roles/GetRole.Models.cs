namespace ESLAdmin.Features.Endpoints.Roles;

//-------------------------------------------------------------------------------
//
//                       class GetRoleRequest
//
//-------------------------------------------------------------------------------
public class GetRoleRequest
{
  public string Name { get; set; }
}

//-------------------------------------------------------------------------------
//
//                       class GetRoleResponse
//
//-------------------------------------------------------------------------------
public class GetRoleResponse
{
  public string Id { get; set; }  
  public string Name { get; set; }
}
