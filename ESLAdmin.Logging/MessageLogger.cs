using ESLAdmin.Logging.Interface;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Logging;

//------------------------------------------------------------------------------
//
//                     class MessageLogger
//
//------------------------------------------------------------------------------
public class MessageLogger : IMessageLogger
{
  ILogger<MessageLogger> _logger;
  public ILogger Logger {  get { return _logger; } }
  //------------------------------------------------------------------------------
  //
  //                     MessageLogger
  //
  //------------------------------------------------------------------------------
  public MessageLogger(ILogger<MessageLogger> logger)
  {
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                     LogController
  //
  //------------------------------------------------------------------------------
  public void LogController(string funcName)
  {
    throw new NotImplementedException();
  }

  //------------------------------------------------------------------------------
  //
  //                     LogController
  //
  //------------------------------------------------------------------------------
  public void LogController<T>(string funcName, T dto)
  {
    throw new NotImplementedException();
  }

  public void LogControllerException(
    string funcName,
    Exception ex)
  {
    _logger.LogError(
      "A Controller exception has occurred. \nFunction: {funcName} \nException: {exception} ",
      funcName,
      ex);
  }

  //------------------------------------------------------------------------------
  //
  //                     LogDatabaseExec
  //
  //------------------------------------------------------------------------------
  public void LogDatabaseExec(
    string funcName,
    string sql,
    string? parameters)
  {
    _logger.LogInformation(
      "Calling exec proc. \nFunction: {funcName} \nSql: {sql} \nParameters: {parameters}",
      funcName,
      sql,
      parameters);
  }

  //------------------------------------------------------------------------------
  //
  //                     LogSqlQuery
  //
  //------------------------------------------------------------------------------
  public void LogSqlQuery(
    string funcName, 
    string sql, 
    string? parameters)
  {
    _logger.LogInformation("Calling sql query. \nFunction: {funcName} \nSql: {sql} \nParameters: {parameters}",
      funcName,
      sql,
      parameters);
  }

  //------------------------------------------------------------------------------
  //
  //                     LogSerializationFailure
  //
  //------------------------------------------------------------------------------
  public void LogSerializationFailure(
      string funcName,
      Exception ex)
  {
    _logger.LogError(
        "Failed to serialize paramaters. \nFunction: {funcName} \nException: {exception}", 
        funcName, 
        ex);
  }

  //------------------------------------------------------------------------------
  //
  //                     LogDatabaseExecSuccess
  //
  //------------------------------------------------------------------------------
  public void LogDatabaseExecSuccess(
    string funcName,
    string sql,
    string? parameters)
  {
    _logger.LogInformation(
      "A database execute command was completed successfully. \nFunction: {funcName} \nSql: {sql} \nParameters: {parameters}", 
      funcName, 
      sql, 
      parameters);
  }

  //------------------------------------------------------------------------------
  //
  //                     LogDatabaseExecFailure
  //
  //------------------------------------------------------------------------------
  public void LogDatabaseExecFailure(
    string funcName,
    string sql,
    string? parameters)
  {
    _logger.LogWarning(
      "A database execute command failure has occured. \nFunction: {funcName} \nSql: {sql} \nParameters: {parameters}", 
      funcName, 
      sql, 
      parameters);
  }

  //------------------------------------------------------------------------------
  //
  //                     LogDatabaseException
  //
  //------------------------------------------------------------------------------
  public void LogDatabaseException(
    string funcName,
    string sql,
    string? parameters,
    Exception ex)
  {
    _logger.LogError(
      "A Database Exception has occured. \nFunction: {funcName} \nSql: {sql} \nParameters: {parameters} \nException {exception}",
        funcName,
        sql,
        parameters,
        ex);
  }

  //------------------------------------------------------------------------------
  //
  //                     LogDatabaseException
  //
  //------------------------------------------------------------------------------
  public void LogDatabaseException(
  string funcName,
  Exception ex)
  {
    _logger.LogError(
    "A Database Exception has occured. \nFunction: {funcName} \nException {exception}",
      funcName,
      ex);
  }

}
