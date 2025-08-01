namespace ESLAdmin.Features.Endpoints.Roles;

//-------------------------------------------------------------------------------
//
//                        class CreateRoleRequest
//
//-------------------------------------------------------------------------------
public class CreateRoleRequest
{
  public required string Name { get; set; }
}

//-------------------------------------------------------------------------------
//
//                        class CreateRoleResponse
//
//-------------------------------------------------------------------------------
public class CreateRoleResponse
{
  public required string Id { get; set; }
  public required string Name { get; set; }
}
