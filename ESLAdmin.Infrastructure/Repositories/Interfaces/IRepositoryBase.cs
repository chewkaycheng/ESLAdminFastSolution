namespace ESLAdmin.Infrastructure.Repositories.Interfaces
{
  public interface IRepositoryBase<ReadT, WriteT> : 
    IRepositoryBaseDapper<ReadT, WriteT>,
    IRepositoryBaseEFCore<ReadT, WriteT>
    where ReadT : class
    where WriteT : class;
}
