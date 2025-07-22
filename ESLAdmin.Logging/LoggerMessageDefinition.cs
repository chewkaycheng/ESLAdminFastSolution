using Microsoft.Extensions.Logging;

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
  [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Exception thrown.")]
  public static partial void LogException(
    this ILogger logger, 
    Exception exception);

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
  [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message ="Database excecute command failed. \n  Sql: {sql}\n  Parameters: {parameters}")]
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
}
