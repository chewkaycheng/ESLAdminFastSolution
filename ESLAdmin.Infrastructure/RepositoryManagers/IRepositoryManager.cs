using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Repositories.Interfaces;

namespace ESLAdmin.Infrastructure.RepositoryManagers
{
  public interface IRepositoryManager
  {
    IChildcareLevelRepository ChildcareLevelRepository { get; }
    IAuthenticationRepository AuthenticationRepository { get; }
  }
}
