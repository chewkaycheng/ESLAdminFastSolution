using Dapper;
using System.Data;

namespace ESLAdmin.Infrastructure.Repositories.Interfaces;

public interface IRepositoryBaseDapper<ReadT, WriteT> 
  where ReadT : class
  where WriteT : class
{
  IEnumerable<ReadT> DapQueryMultiple(
      string sql,
      DynamicParameters? parameters,
      CommandType commandType = CommandType.StoredProcedure);

  ReadT? DapQuerySingle(
    string sql,
    DynamicParameters? parameters,
    CommandType commandType = CommandType.StoredProcedure);

  Task<IEnumerable<ReadT>> DapQueryMultipleAsync(
      string sql,
      DynamicParameters? parameters,
      CommandType commandType = CommandType.StoredProcedure);

  Task<ReadT?> DapQuerySingleAsync(
  string sql,
  DynamicParameters? parameters,
  CommandType commandType = CommandType.StoredProcedure);

  bool DapExecWithTrans(
    string sql,
    DynamicParameters parameters,
    CommandType commandType = CommandType.StoredProcedure);

  Task<bool> DapExecWithTransAsync(
    string sql,
    DynamicParameters parameters,
    CommandType commandType = CommandType.StoredProcedure);

}
