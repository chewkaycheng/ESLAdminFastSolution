using ESLAdmin.Infrastructure.Persistence.DatabaseContexts;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

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
  protected IDbContextDapper _dbContextDapper;
  protected DbContextEF? _dbContextEF;
  //protected IMessageLogger _messageLogger;
  protected ILogger _logger;

  //------------------------------------------------------------------------------
  //
  //                       RepositoryBase
  //
  //------------------------------------------------------------------------------
  public RepositoryBase(
    IDbContextDapper dbContextDapper,
    ILogger logger,
    DbContextEF? dbContextEF = null)
  {
    _dbContextDapper = dbContextDapper;
    _dbContextEF = dbContextEF;
    _logger = logger;
    //_messageLogger = messageLogger;
  }
}
