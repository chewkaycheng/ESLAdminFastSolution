using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;

namespace ESLAdmin.Features.ChildcareLevels.Repositories;

public class ChildcareLevelRepositoryManager : IChildcareLevelRepositoryManager
{
  private readonly Lazy<IChildcareLevelRepository> _childcareLevelRepository;

  public ChildcareLevelRepositoryManager(
    IDbContextDapper dbContextDapper,
    IMessageLogger messageLogger)
  {
    _childcareLevelRepository =
      new Lazy<IChildcareLevelRepository>(() =>
        new ChildcareLevelRepository(
          dbContextDapper,
          null,
          messageLogger));
  }

  public IChildcareLevelRepository ChildcareLevel =>
    _childcareLevelRepository.Value;
}


