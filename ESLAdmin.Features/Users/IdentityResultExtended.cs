using ESLAdmin.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Features.Users
{
  public class IdentityResultExtended : IdentityResult
  {
    public IdentityResult IdentityResult { get; set; }
    public User User { get; set; }
  }
}
