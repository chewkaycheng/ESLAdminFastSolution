using Dapper;
using ErrorOr;
using ESLAdmin.Common.Errors;
using ESLAdmin.Infrastructure.Configuration;
using ESLAdmin.Infrastructure.Data.Interfaces;
using ESLAdmin.Logging;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.Json;

namespace ESLAdmin.Infrastructure.Data;

//------------------------------------------------------------------------------
//
//                       class DbContextDapper
//
//------------------------------------------------------------------------------
public class DbContextDapper : IDbContextDapper
{
  private readonly ILogger<DbContextDapper> _logger;
  private readonly string _connectionString;

  //------------------------------------------------------------------------------
  //
  //                       DbContextDapper
  //
  //------------------------------------------------------------------------------
  public DbContextDapper(
    ILogger<DbContextDapper> logger,
    IConfigurationParams configParams)
  {
    _logger = logger;
    _connectionString = configParams.Settings["ConnectionStrings:ESLAdminConnection"]!;
  }

  //------------------------------------------------------------------------------
  //
  //                       GetConnection
  //
  //------------------------------------------------------------------------------
  public IDbConnection GetConnection()
  {
    return new FbConnection(_connectionString);
  }

  //------------------------------------------------------------------------------
  //
  //                       GetConnectionAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<IDbConnection>> GetConnectionAsync()
  {
    try
    {
      _logger.LogDebug("Attempting to open database connection.");
      var connection = new FbConnection(_connectionString);
      await connection.OpenAsync();
      _logger.LogDebug("Database connection opened successfully.");
      return connection;
    }
    catch (FbException ex)
    {
      _logger.LogException(ex);
      return Errors.DatabaseErrors.DatabaseConnectionError(ex.Message);
    }
    catch (ArgumentException ex)
    {
      _logger.LogException(ex);
      return Errors.DatabaseErrors.InvalidConnectionString(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogException(ex);
      return Errors.DatabaseErrors.InvalidOperation(ex.Message);
    }
    catch (OperationCanceledException ex)
    {
      _logger.LogException(ex);
      return Errors.DatabaseErrors.OperationCanceled(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return Errors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       GetTransactionAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<IDbTransaction>> GetTransactionAsync(
    IDbConnection connection, CancellationToken cancellationToken = default)
  {
    if (connection == null)
    {
      _logger.LogError("GetTransactionAsync: Connection is null.");
      return Errors.DatabaseErrors.ConnectionNullError();
    }

    if (connection is not FbConnection fbConnection)
    {
      _logger.LogError("GetTransactionAsync: Connection is not a Firebird connection. Type: {ConnectionType}", connection.GetType().Name);
      return Errors.DatabaseErrors.InvalidConnectionType("Connection must be a Firebird connection.");
    }

    try
    {
      if (connection.State != ConnectionState.Open)
      {
        await ((FbConnection)connection).OpenAsync(cancellationToken);
      }

      return await ((FbConnection)connection).BeginTransactionAsync(cancellationToken);
    }
    catch (FbException ex)
    {
      _logger.LogException(ex);
      return Errors.DatabaseErrors.DatabaseConnectionError($"Firebird error: {ex.Message} (ErrorCode: {ex.ErrorCode})");
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogException(ex);
      return Errors.DatabaseErrors.InvalidOperation(ex.Message);
    }
    catch (OperationCanceledException ex)
    {
      _logger.LogException(ex);
      return Errors.DatabaseErrors.OperationCanceled(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return Errors.CommonErrors.Exception(ex.Message);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                       SerializeDynamicParameters
  //
  //------------------------------------------------------------------------------
  public string SerializeDynamicParameters(
    DynamicParameters parameters)
  {
    if (parameters == null)
      return "{}";

    var paramDict = new Dictionary<string, object>();

    foreach (var paramName in parameters.ParameterNames)
    {
      var value = parameters.Get<object>(paramName);
      paramDict[paramName] = value ?? "NULL";
    }

    try
    {
      return JsonSerializer.Serialize(
        paramDict,
        new JsonSerializerOptions
        {
          WriteIndented = true,
          Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        });
    }
    catch (Exception ex)
    {
      _logger.LogSerializationFailure(
        ex);
      return "";
    }
  }
}
