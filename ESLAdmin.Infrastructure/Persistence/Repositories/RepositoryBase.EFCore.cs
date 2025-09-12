using ErrorOr;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging;
using Microsoft.EntityFrameworkCore;

namespace ESLAdmin.Features.Repositories;

//------------------------------------------------------------------------------
//
//                       class RepositoryBase
//
//------------------------------------------------------------------------------
public partial class RepositoryBase<ReadT, WriteT> :
  IRepositoryBase<ReadT, WriteT>
  where ReadT : class
  where WriteT : class
{
  //------------------------------------------------------------------------------
  //
  //                       FindAll
  //
  //------------------------------------------------------------------------------
  public ErrorOr<IQueryable<ReadT>> FindAll(bool trackChanges)
  {
    try
    {
      if (_dbContextEF == null)
      {
        throw new ArgumentNullException(nameof(_dbContextEF),
          "Entity Framework DbContext is not provided.");
      }

      IQueryable<ReadT> ret;
      if (!trackChanges)
      {
        ret = _dbContextEF.Set<ReadT>().AsNoTracking();
        return ErrorOrFactory.From(ret);
      }
      ret = _dbContextEF.Set<ReadT>();
      return ErrorOrFactory.From(ret);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
        .HandleException(ex, _logger);
    }
  }

}
