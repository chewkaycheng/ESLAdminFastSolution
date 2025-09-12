using ErrorOr;

namespace ESLAdmin.Infrastructure.Repositories.Interfaces;

public interface IRepositoryBaseEFCore<ReadT, WriteT>
  where ReadT : class
  where WriteT : class
{
  ErrorOr<IQueryable<ReadT>> FindAll(bool trackChanges);
}
