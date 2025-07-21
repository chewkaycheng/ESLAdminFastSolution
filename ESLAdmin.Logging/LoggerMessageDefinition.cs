using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
  //                     LogCreateUserRequest
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Function entry.\n=>Request: \\n    Username: '{userName}', FirstName: '{firstName}', LastName: '{lastName}', Email: '{email}'\\n    Password: '[Hidden]', PhoneNumber: '{phoneNumber}', Roles: '{roles}'", SkipEnabledCheck = true)]
  public static partial void LogCreateUserRequest(
    this ILogger logger,
    string userName,
    string firstName,
    string lastName,
    string email,
    string phoneNumber,
    string roles);

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
  //                       LogFunctionExit
  //
  //------------------------------------------------------------------------------
  [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "Exit function. {context}")]
  public static partial void LogFunctionExit(
    this ILogger logger,
    string? context = null);
}
