using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Features.Repositories.Interfaces
{
  public interface IRepositoryBase<ReadT, WriteT> : 
    IRepositoryBaseDapper<ReadT, WriteT>,
    IRepositoryBaseEFCore<ReadT, WriteT>
    where ReadT : class
    where WriteT : class;
}
