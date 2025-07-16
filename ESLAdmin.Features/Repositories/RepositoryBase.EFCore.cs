using ESLAdmin.Features.Repositories.Interfaces;

namespace ESLAdmin.Features.Repositories
{
  public partial class RepositoryBase<ReadT, WriteT> :
    IRepositoryBaseEFCore<ReadT, WriteT>,
    IRepositoryBaseDapper<ReadT, WriteT>
    where ReadT : class
    where WriteT : class
  {
  }
}
