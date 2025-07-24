using ESLAdmin.Infrastructure.Data.Interfaces;
using ESLAdmin.Infrastructure.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;

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
  protected IDbContextDapper? _dbContextDapper;
  //protected IDbContextEF? _dbContextEF;
  protected IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                       RepositoryBase
  //
  //------------------------------------------------------------------------------
  public RepositoryBase(
    IDbContextDapper? dbContextDapper,
    //IDbContextEF? dbContextEF,
    IMessageLogger messageLogger)
  {
    _dbContextDapper = dbContextDapper;
    //_dbContextEF = dbContextEF;
    _messageLogger = messageLogger;
  }
}
