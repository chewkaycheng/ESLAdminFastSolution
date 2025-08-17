using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
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
  //protected IDbContextEF? _dbContextEF;
  protected IMessageLogger _messageLogger;
  protected ILogger _logger;

  //------------------------------------------------------------------------------
  //
  //                       RepositoryBase
  //
  //------------------------------------------------------------------------------
  public RepositoryBase(
    IDbContextDapper dbContextDapper,
    ILogger logger,
    IMessageLogger messageLogger)
  {
    _dbContextDapper = dbContextDapper;
    _logger = logger;
    _messageLogger = messageLogger;
  }
}
