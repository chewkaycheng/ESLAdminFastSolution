using Dapper;
using ErrorOr;
using System.Data;

namespace ESLAdmin.Infrastructure.Repositories.Interfaces;

public interface IRepositoryBaseDapper<ReadT, WriteT>
  where ReadT : class
  where WriteT : class
{
  ErrorOr<IEnumerable<ReadT>> DapQueryMultiple(
      string sql,
      DynamicParameters? parameters,
      CommandType commandType = CommandType.StoredProcedure);

  ReadT? DapQuerySingle(
    string sql,
    DynamicParameters? parameters,
    CommandType commandType = CommandType.StoredProcedure);

  Task<ErrorOr<IEnumerable<ReadT>>> DapQueryMultipleAsync(
      string sql,
      DynamicParameters? parameters,
      CommandType commandType = CommandType.StoredProcedure,
      CancellationToken cancellationToken = default);

  Task<ErrorOr<ReadT?>> DapQuerySingleAsync(
  string sql,
  DynamicParameters? parameters,
  CommandType commandType = CommandType.StoredProcedure);

  bool DapExecWithTrans(
    string sql,
    DynamicParameters parameters,
    CommandType commandType = CommandType.StoredProcedure);

  Task<ErrorOr<bool>> DapExecWithTransAsync(
    string sql,
    DynamicParameters parameters,
    CommandType commandType = CommandType.StoredProcedure,
    CancellationToken cancellationToken = default);

}
