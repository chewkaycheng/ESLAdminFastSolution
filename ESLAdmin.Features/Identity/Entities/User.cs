using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Features.Identity.Entities;

//------------------------------------------------------------------------------
//
//                        Class User
//
//------------------------------------------------------------------------------
public class User : IdentityUser
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
}
