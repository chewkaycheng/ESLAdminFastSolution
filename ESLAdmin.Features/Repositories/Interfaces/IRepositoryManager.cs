using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Users.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Features.Repositories.Interfaces
{
  public interface IRepositoryManager
  {
    IChildcareLevelRepository ChildcareLevelRepository { get; }
    IAuthenticationRepository AuthenticationRepository { get; }
  }
}
