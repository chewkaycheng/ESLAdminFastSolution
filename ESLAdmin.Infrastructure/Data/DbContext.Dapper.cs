using Dapper;
using ESLAdmin.Common.Exceptions;
using ESLAdmin.Infrastructure.Data.Interfaces;
using ESLAdmin.Logging.Interface;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
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
  private readonly IMessageLogger _messageLogger;
  private readonly string _connectionString;

  //------------------------------------------------------------------------------
  //
  //                       DbContextDapper
  //
  //------------------------------------------------------------------------------
  public DbContextDapper(
    IMessageLogger messageLogger,
    IConfiguration configuration)
  {
    _messageLogger = messageLogger;

    _connectionString = configuration.GetConnectionString("ESLAdminConnection") 
      ?? throw new NullOrEmptyException("Connection string", nameof(DbContextDapper));

    if (string.IsNullOrEmpty(_connectionString)) 
      throw new NullOrEmptyException("Connection string", nameof(DbContextDapper));
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
  public async Task<IDbConnection> GetConnectionAsync()
  {
    var connection = new FbConnection(_connectionString);
    await connection.OpenAsync();
    return connection;
  }

  //------------------------------------------------------------------------------
  //
  //                       GetTransactionAsync
  //
  //------------------------------------------------------------------------------
  public async Task<IDbTransaction> GetTransactionAsync(
    IDbConnection connection)
  {
    if (connection == null)
      throw new ArgumentNullException(
        string.Format("Database connection cannot be null. \nfuncName: {0}",
          nameof(GetTransactionAsync)));

    if (connection.State != ConnectionState.Open)
    {
      await ((FbConnection)connection).OpenAsync();
    }

    return await ((FbConnection)connection).BeginTransactionAsync();
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
      _messageLogger.LogSerializationFailure(
        nameof(SerializeDynamicParameters),
        ex);
      return "";
    }
  }
}
