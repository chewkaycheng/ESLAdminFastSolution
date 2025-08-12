using Dapper;
using ErrorOr;
using ESLAdmin.Common.Errors;
using ESLAdmin.Common.Exceptions;
using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Infrastructure.Repositories.Interfaces;
using ESLAdmin.Logging;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Logging;
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
  public ErrorOr<IEnumerable<ReadT>> DapQueryMultiple(
    string sql,
    DynamicParameters? parameters,
    CommandType commandType = CommandType.StoredProcedure)
  {
    _logger.LogFunctionEntry();
    var result = _dbContextDapper.GetConnection();
    if (result.IsError)
    {
      return result.Errors;
    }
    using IDbConnection connection = result.Value;

    var results =  connection.Query<ReadT>(
      sql,
      parameters,
      commandType: commandType);
    _logger.LogFunctionExit();
    return ErrorOrFactory.From(results);
  }

  //------------------------------------------------------------------------------
  //
  //                       DapQueryMultipleAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<IEnumerable<ReadT>>> DapQueryMultipleAsync(
    string sql,
    DynamicParameters? parameters,
    CommandType commandType = CommandType.StoredProcedure,
    CancellationToken cancellationToken = default)
  {
    _logger.LogFunctionEntry();
    var connectionResult = await _dbContextDapper.GetConnectionAsync();
    if (connectionResult.IsError)
    {
      return connectionResult.Errors;
    }
    using IDbConnection connection = connectionResult.Value;

    var result = await connection.QueryAsync<ReadT>(
          sql,
          parameters,
          commandType: commandType);
    _logger.LogFunctionExit();
    return ErrorOrFactory.From(result);
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
  public async Task<ErrorOr<ReadT?>> DapQuerySingleAsync(
    string sql,
    DynamicParameters? parameters,
    CommandType commandType = CommandType.StoredProcedure)
  {
    var connectionResult = await _dbContextDapper.GetConnectionAsync();
    if (connectionResult.IsError)
    {
      return connectionResult.Errors;
    }
    using IDbConnection connection = connectionResult.Value;
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
  public async Task<ErrorOr<bool>> DapExecWithTransAsync(
    string sql,
    DynamicParameters parameters,
    CommandType commandType = CommandType.StoredProcedure,
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(sql))
    {
      _logger.LogError("DapExecWithTransAsync: SQL query or stored procedure name is null or empty.");
      return Errors.DatabaseErrors.InvalidSqlQuery("SQL query or stored procedure name cannot be null or empty.");
    }

    if (parameters == null)
    {
      _logger.LogError("DapExecWithTransAsync: Parameters are null.");
      return Errors.DatabaseErrors.InvalidParameters("Parameters cannot be null.");
    }

    var connectionResult = await _dbContextDapper.GetConnectionAsync();
    if (connectionResult.IsError)
    {
      _logger.LogWarning(
        "DapExecWithTransAsync: Failed to get connection: {Error}",
        connectionResult.FirstError.Description);
      return connectionResult.Errors;
    }
    using IDbConnection connection = connectionResult.Value;

    var transactionResult = await _dbContextDapper.GetTransactionAsync(connection);
    if (transactionResult.IsError)
    {
      _logger.LogWarning(
        "DapExecWithTransAsync: Failed to begin transaction: {Error}",
        transactionResult.FirstError.Description);
      return transactionResult.Errors;
    }

    using IDbTransaction transaction = transactionResult.Value;

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
        await ((FbTransaction)transaction).CommitAsync(cancellationToken);

        _logger.LogDatabaseExecSuccess(
          sql,
          _dbContextDapper.SerializeDynamicParameters(parameters));

        return true;
      }
      else
      {
        _logger.LogDatabaseExecFailure(
          sql,
          _dbContextDapper.SerializeDynamicParameters(parameters));

        await ((FbTransaction)transaction).RollbackAsync();
        return Errors.DatabaseErrors.StoredProcedureError(sql, dbApiError);
      }
    }
    catch (Exception ex)
    {
      await ((FbTransaction)transaction).RollbackAsync();

      _logger.LogDatabaseException(
        sql,
        _dbContextDapper.SerializeDynamicParameters(parameters),
        ex);

      return Errors.CommonErrors.Exception(ex.Message);
    }
  }
}
