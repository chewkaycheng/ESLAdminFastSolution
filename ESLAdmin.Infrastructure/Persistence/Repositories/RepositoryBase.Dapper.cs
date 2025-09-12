using Dapper;
using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
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
    try
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
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
        .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       DapQuerySingle
  //
  //------------------------------------------------------------------------------
  public ErrorOr<ReadT?> DapQuerySingle(
    string sql,
    DynamicParameters? parameters,
    CommandType commandType = CommandType.StoredProcedure)
  {

    var result = _dbContextDapper.GetConnection();
    if (result.IsError)
    {
      return result.Errors;
    }
    using IDbConnection connection = result.Value;

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
    try
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
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
        .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       DapExecWithTrans
  //
  //------------------------------------------------------------------------------
  public ErrorOr<bool> DapExecWithTrans(
    string sql,
    DynamicParameters parameters,
    CommandType commandType = CommandType.StoredProcedure)
  {
    parameters.AddInt32OutputParam(
      "dbApiError");

    var connectionResult = _dbContextDapper.GetConnection();
    if (connectionResult.IsError)
    {
      return connectionResult.Errors;
    }
    using IDbConnection connection = connectionResult.Value;
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

        _logger.LogDatabaseExecSuccess(
          sql,
          _dbContextDapper.SerializeDynamicParameters(parameters));

        return true;
      }
      else
      {
        transaction.Rollback();

        _logger.LogDatabaseExecSuccess(
          sql,
          _dbContextDapper.SerializeDynamicParameters(parameters));

        return false;
      }
    }
    catch (Exception ex)
    {
      _logger.LogDatabaseException(
        sql,
        _dbContextDapper.SerializeDynamicParameters(parameters),
        ex);

      transaction.Rollback();
      return AppErrors.CommonErrors.Exception(ex.Message);
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
      return AppErrors.DatabaseErrors.InvalidSqlQuery("SQL query or stored procedure name cannot be null or empty.");
    }

    if (parameters == null)
    {
      _logger.LogError("DapExecWithTransAsync: Parameters are null.");
      return AppErrors.DatabaseErrors.InvalidParameters("Parameters cannot be null.");
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
        DbConstsOperationResult.DBAPIERROR
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
        //return AppErrors.DatabaseErrors.StoredProcedureError(sql, dbApiError);
        return false;
      }
    }
    catch (Exception ex)
    {
      await ((FbTransaction)transaction).RollbackAsync();

      _logger.LogDatabaseException(
        sql,
        _dbContextDapper.SerializeDynamicParameters(parameters),
        ex);

      return DatabaseExceptionHandler
        .HandleException(ex, _logger);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetAllAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<IEnumerable<ReadT>>> GetAllAsync(string sql)
  {
    var result = await DapQueryMultipleAsync(sql, null);
    if (result.IsError)
    {
      return result.Errors;
    }

    var entity = result.Value;
    return ErrorOrFactory.From(entity);
  }

  //------------------------------------------------------------------------------
  //
  //                       GetSingleAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<ReadT?>> GetSingleAsync(
    string sql,
    DynamicParameters parameters)
  {
    var result = await DapQuerySingleAsync(
      sql,
      parameters);
    if (result.IsError)
    {
      return result.Errors;
    }

    return result.Value;
  }

  //------------------------------------------------------------------------------
  //
  //                       CreateAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> PersistAsync(
    string sql,
    DynamicParameters parameters)
  {
    var result = await DapExecWithTransAsync(sql, parameters);
    if (result.IsError)
    {
      return result.Errors;
    }

    return new Success();
  }
  
}
