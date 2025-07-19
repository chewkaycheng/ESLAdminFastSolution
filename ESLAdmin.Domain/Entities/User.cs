using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Domain.Entities;

//------------------------------------------------------------------------------
//
//                        Class User
//
//------------------------------------------------------------------------------
public class User : IdentityUser
{
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
}
