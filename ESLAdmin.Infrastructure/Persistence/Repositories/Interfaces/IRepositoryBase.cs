using ESLAdmin.Infrastructure.Repositories.Interfaces;

namespace ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces
{
  public interface IRepositoryBase<ReadT, WriteT> :
    IRepositoryBaseDapper<ReadT, WriteT>,
    IRepositoryBaseEFCore<ReadT, WriteT>
    where ReadT : class
    where WriteT : class;
}
