using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Common.CustomErrors;

public static partial class AppErrors
{
  //-------------------------------------------------------------------------------
  //
  //                       class DatabaseErrors
  //
  //-------------------------------------------------------------------------------\
  public static class DatabaseErrors
  {
    //-------------------------------------------------------------------------------
    //
    //                       ConnectionNullError
    //
    //-------------------------------------------------------------------------------
    public static Error ConnectionNullError() =>
      Error.Failure(
        code: "Database.ConnectionNullError",
        description: "Database connection cannot be null.");

    //-------------------------------------------------------------------------------
    //
    //                       DatabaseConnectionError
    //
    //-------------------------------------------------------------------------------
    public static Error DatabaseConnectionError(string message) =>
      Error.Failure(
        code: "Database.DatabaseConnectionError",
        description: $"A database connection error has occurred: {message}");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidConnectionString
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidConnectionString(string message) =>
      Error.Failure(
        code: "Database.InvalidConnectionString",
        description: $"Connection string is invalid: {message}");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidOperation
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidOperation(string message) =>
      Error.Failure(
        code: "Database.InvalidOperation",
        description: $"An invalid operation has occurred: {message}");

    //-------------------------------------------------------------------------------
    //
    //                       OperationCanceled
    //
    //-------------------------------------------------------------------------------
    public static Error OperationCanceled(string message) =>
      Error.Failure(
        code: "Database.OperationCanceled",
        description: $"Operaion has been cancelled: {message}");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidConnectionType
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidConnectionType(string message) =>
      Error.Failure(
        code: "Database.InvalidConnectionType",
        description: $"{message}");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidSqlQuery
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidSqlQuery(string message) =>
      Error.Failure(
        code: "Database.InvalidSqlQuery",
        description: $"{message}");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidParameters
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidParameters(string message) =>
      Error.Failure(
        code: "Database.InvalidParameters",
        description: $"{message}");

    //-------------------------------------------------------------------------------
    //
    //                       StoredProcedureError
    //
    //-------------------------------------------------------------------------------
    public static Error StoredProcedureError(string procedureName, int errorCode) =>
        Error.Failure(
          "Database.StoredProcedureError", 
          $"Stored procedure '{procedureName}' failed with error code: {errorCode}");

    //-------------------------------------------------------------------------------
    //
    //                       DatabaseError
    //
    //-------------------------------------------------------------------------------
    public static Error DatabaseError(string description) =>
        Error.Failure(
          "Database.DatabaseError", 
          description);

    //-------------------------------------------------------------------------------
    //
    //                       OperationCanceled
    //
    //-------------------------------------------------------------------------------
    public static Error OperationCanceled() =>
        Error.Failure(
          "Database.OperationCanceled", 
          "Operation was canceled");
  }
}