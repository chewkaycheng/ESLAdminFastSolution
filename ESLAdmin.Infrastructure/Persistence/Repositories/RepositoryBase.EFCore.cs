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
}
