using ESLAdmin.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Features.Users;

public class IdentityResultExtended : IdentityResult
{
  public IdentityResult IdentityResult { get; set; }
  public User User { get; set; }
}
