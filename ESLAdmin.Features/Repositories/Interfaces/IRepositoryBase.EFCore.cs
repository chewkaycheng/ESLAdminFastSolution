namespace ESLAdmin.Features.Repositories.Interfaces;

public interface IRepositoryBaseEFCore<ReadT, WriteT>
  where ReadT : class
  where WriteT : class
{
}
