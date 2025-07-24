using Dapper;
using ESLAdmin.Common.Exceptions;
using ESLAdmin.Features.Exceptions;
using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Infrastructure.Repositories.Interfaces;
using FirebirdSql.Data.FirebirdClient;
using System.Data;

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
  //                       DapQueryMultiple
  //
  //------------------------------------------------------------------------------
  public IEnumerable<ReadT> DapQueryMultiple(
    string sql,
    DynamicParameters? parameters, 
    CommandType commandType = CommandType.StoredProcedure)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(DapQueryMultiple), 
        "_dbcontextDapper");

    using IDbConnection connection = _dbContextDapper.GetConnection();

    return connection.Query<ReadT>(
      sql,
      parameters,
      commandType: commandType);
  }

  //------------------------------------------------------------------------------
  //
  //                       DapQueryMultipleAsync
  //
  //------------------------------------------------------------------------------
  public async Task<IEnumerable<ReadT>> DapQueryMultipleAsync(
    string sql, 
    DynamicParameters? parameters, 
    CommandType commandType = CommandType.StoredProcedure)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(DapQueryMultipleAsync), 
        "_dbcontextDapper");

    using IDbConnection connection = await _dbContextDapper.GetConnectionAsync();

    return await connection.QueryAsync<ReadT>(
          sql,
          parameters,
          commandType: commandType);
  }

  //------------------------------------------------------------------------------
  //
  //                       DapQuerySingle
  //
  //------------------------------------------------------------------------------
  public ReadT? DapQuerySingle(
    string sql, 
    DynamicParameters? parameters, 
    CommandType commandType = CommandType.StoredProcedure)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(DapQuerySingle), 
        "_dbcontextDapper");

    using IDbConnection connection = _dbContextDapper.GetConnection();

    return connection.QueryFirstOrDefault<ReadT>(
          sql,
          parameters,
          commandType: commandType);
  }

  //------------------------------------------------------------------------------
  //
  //                       DapQuerySingleAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ReadT?> DapQuerySingleAsync(
    string sql, 
    DynamicParameters? parameters, 
    CommandType commandType = CommandType.StoredProcedure)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(DapQueryMultipleAsync), 
        "_dbcontextDapper");

    using IDbConnection connection = await _dbContextDapper.GetConnectionAsync();
    return await connection.QueryFirstOrDefaultAsync<ReadT>(
          sql,
          parameters,
          commandType: commandType);
  }

  //------------------------------------------------------------------------------
  //
  //                       DapExecWithTrans
  //
  //------------------------------------------------------------------------------
  public bool DapExecWithTrans(
    string sql,
    DynamicParameters parameters,
    CommandType commandType = CommandType.StoredProcedure)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(DapExecWithTrans), 
        "_dbcontextDapper");

    parameters.AddInt32OutputParam(
      "dbApiError");

    using IDbConnection connection = _dbContextDapper.GetConnection();
    using IDbTransaction transaction = connection.BeginTransaction();

    try
    {
      connection.Execute(
        sql,
        parameters,
        transaction,
        commandType: commandType);

      var dbApiError = parameters.Get<int>(
        "dbApiError"
      );

      if (dbApiError == 0)
      {
        transaction.Commit();

        _messageLogger.LogDatabaseExecSuccess(
          nameof(DapExecWithTrans),
          sql,
          _dbContextDapper.SerializeDynamicParameters(parameters));

        return true;
      }
      else
      {
        transaction.Rollback();

        _messageLogger.LogDatabaseExecSuccess(
          nameof(DapExecWithTrans),
          sql,
          _dbContextDapper.SerializeDynamicParameters(parameters));

        return false;
      } 
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(DapExecWithTrans),
        sql,
        _dbContextDapper.SerializeDynamicParameters(parameters),
        ex);

      transaction.Rollback();
      throw new DatabaseException(
        nameof(DapExecWithTrans),
        sql,
        _dbContextDapper.SerializeDynamicParameters(parameters),
        ex);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       DapExecWithTransAsync
  //
  //------------------------------------------------------------------------------
  public async Task<bool> DapExecWithTransAsync(
    string sql,
    DynamicParameters parameters,
    CommandType commandType = CommandType.StoredProcedure)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(DapExecWithTrans), 
        "_dbcontextDapper");

    using IDbConnection connection 
      = await _dbContextDapper.GetConnectionAsync();
    using IDbTransaction transaction = 
      await _dbContextDapper.GetTransactionAsync(connection);

    try
    {
      await connection.ExecuteAsync(
              sql,
              parameters,
              transaction,
              commandType: commandType);

      var dbApiError = parameters.Get<int>(
        OperationResultConsts.DBAPIERROR
      );

      if (dbApiError == 0)
      {
        await ((FbTransaction) transaction).CommitAsync();

        _messageLogger.LogDatabaseExecSuccess(
          nameof(DapExecWithTransAsync),
          sql,
          _dbContextDapper.SerializeDynamicParameters(parameters));

        return true;
      }
      else
      {
        _messageLogger.LogDatabaseExecFailure(
          nameof(DapExecWithTransAsync),
          sql,
          _dbContextDapper.SerializeDynamicParameters(parameters));

        await ((FbTransaction)transaction).RollbackAsync();
        return false;
      }
    }
    catch (Exception ex)
    {
      await ((FbTransaction)transaction).RollbackAsync();

      _messageLogger.LogDatabaseException(
        nameof(DapExecWithTransAsync),
        sql,
        _dbContextDapper.SerializeDynamicParameters(parameters),
        ex);

      throw new DatabaseException(
        nameof(DapExecWithTrans),
        sql,
        _dbContextDapper.SerializeDynamicParameters(parameters),
        ex);
    }
  }
}
