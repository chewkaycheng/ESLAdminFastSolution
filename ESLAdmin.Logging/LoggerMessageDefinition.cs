using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ESLAdmin.Logging;

//------------------------------------------------------------------------------
//
//                      class MessageLoggerDefs
//
//------------------------------------------------------------------------------
public static partial class MessageLoggerDefs
{
  //------------------------------------------------------------------------------
  //
  //                     LogException
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Exception thrown. TraceId: '{traceId}'.")]
  public static partial void LogException(
    this ILogger logger,
    Exception exception,
    string traceId);

  //------------------------------------------------------------------------------
  //
  //                     LogException
  //
  //------------------------------------------------------------------------------
  public static void LogException(this ILogger logger, Exception exception)
  {
    logger.LogException(exception, Activity.Current?.Id ?? "NoTraceId");
  }

  //------------------------------------------------------------------------------
  //
  //                     LogDatabaseExecSuccess
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Database execute command successful. \n  Sql: {sql}\n  Parameters: {parameters}")]
  public static partial void LogDatabaseExecSuccess(
    this ILogger logger,
    string sql,
    string parameters);

  //------------------------------------------------------------------------------
  //
  //                     LogDatabaseExecFailure
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Database excecute command failed. \n  Sql: {sql}\n  Parameters: {parameters}")]
  public static partial void LogDatabaseExecFailure(
    this ILogger logger,
    string sql,
    string parameters,
    Exception exception);

  //------------------------------------------------------------------------------
  //
  //                       LogValidationErrors
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Validation Errors.\n=>Errors:{validationErrors}", SkipEnabledCheck = true)]
  public static partial void LogValidationErrors(
    this ILogger logger,
    string validationErrors);

  //------------------------------------------------------------------------------
  //
  //                       LogFunctionEntry
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "Function entry. {context}")]
  public static partial void LogFunctionEntry(
    this ILogger logger,
    string? context = null);


  //------------------------------------------------------------------------------
  //
  //                       LogFunctionExit
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "Exit function. {context}")]
  public static partial void LogFunctionExit(
    this ILogger logger,
    string? context = null);

  //------------------------------------------------------------------------------
  //
  //                       LogNotFound
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "The {entityName} with {context} is not found.")]
  public static partial void LogNotFound(
    this ILogger logger,
    string entityName,
    string? context = null);

  //------------------------------------------------------------------------------
  //
  //                       LogIdentityErrors
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "Identity Error performing {identityFunction}. Id: {id}. {context}")]
  public static partial void LogIdentityErrors(
    this ILogger logger,
    string identityFunction,
    string? id,
    string context);
  //------------------------------------------------------------------------------
  //
  //                       LogIdentityErrors
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "A database execute command failure has occured. \nSql: {sql} \nParameters: {parameters}")]
  public static partial void LogDatabaseExecFailure(
    this ILogger logger,
    string sql,
    string? parameters);

  //------------------------------------------------------------------------------
  //
  //                     LogDatabaseException
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "A Database Exception has occured. \nSql: '{sql}' \nParameters: '{parameters}' \nException: '{ex}'")]
  public static partial void LogDatabaseException(
    this ILogger logger,
    string sql,
    string? parameters,
    Exception ex);

  //------------------------------------------------------------------------------
  //
  //                     LogSerializationFailure
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "Failed to serialize paramaters.\nException: '{ex}'")]
  public static partial void LogSerializationFailure(
      this ILogger logger,
      Exception ex);

  //------------------------------------------------------------------------------
  //
  //                     LogCustomError
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 11, Level = LogLevel.Error, Message = "{message}")]
  public static partial void LogCustomError(
    this ILogger logger,
    string message);

  //------------------------------------------------------------------------------
  //
  //                     LogCustomInformation
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 12, Level = LogLevel.Information, Message = "{message}")]
  public static partial void LogCustomInformation(
    this ILogger logger,
    string message);

  //------------------------------------------------------------------------------
  //
  //                     LogMissingConfigKey
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(
    EventId = 13, 
    Level = LogLevel.Error, 
    Message = "Configuration key: '{key}' is not found in the configuration file.")]
  public static partial void LogMissingConfigKey(
    this ILogger logger,
    string key);
}
