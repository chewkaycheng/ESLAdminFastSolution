using ESLAdmin.Features.ChildcareLevels.Repositories;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;

namespace ESLAdmin.Features.Repositories;

public class RepositoryManager : IRepositoryManager
{
  private readonly Lazy<IChildcareLevelRepository> _childcareLevelRepository;
  public RepositoryManager(
    IDbContextDapper dbContextDapper,
    IMessageLogger messageLogger)
  {
    _childcareLevelRepository = new Lazy<IChildcareLevelRepository>(
      () => new ChildcareLevelRepository(
        dbContextDapper,
        messageLogger));
  }
  public IChildcareLevelRepository ChildcareLevelRepository => 
    _childcareLevelRepository.Value;
}


