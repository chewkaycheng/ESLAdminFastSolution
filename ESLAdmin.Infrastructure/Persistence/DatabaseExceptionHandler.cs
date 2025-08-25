using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Logging;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Persistence;

//------------------------------------------------------------------------------
//
//                      static class DatabaseExceptionHandler
//
//------------------------------------------------------------------------------
public static class DatabaseExceptionHandler
{
  //------------------------------------------------------------------------------
  //
  //                      HandleException
  //
  //------------------------------------------------------------------------------
  public static Error HandleException(
    Exception ex,
    ILogger logger)
  {
    logger.LogException(ex);

    return ex switch
    {

      ArgumentNullException ane => AppErrors
        .IdentityExceptions
        .InvalidArgumentException(ane.Message),
      DbUpdateException dbu => AppErrors.DatabaseExceptions.DatabaseException(
        ex.InnerException?.Message ?? ex.Message),
      FbException fbe => ParseAndCreate(fbe.Message, fbe.ErrorCode),
      ObjectDisposedException ode => 
        AppErrors.DatabaseExceptions.DatabaseException(
          $"Internal server error: Resource unavailable. {ode.Message}"),
      InvalidOperationException ioe => AppErrors
        .IdentityExceptions
        .InvalidOperationException(ioe.Message),
      OperationCanceledException oce => AppErrors.DatabaseErrors.OperationCanceled(),
      Exception exception => AppErrors.CommonErrors.Exception(exception.Message)
    };
  }
  private static Error ParseAndCreate(string message, int errorCode)
  {
    var messageLines = message.Split(new[] { "\r\n" }, StringSplitOptions.None);
    return AppErrors.DatabaseExceptions.DatabaseException(
      $"Firebird errors:  {string.Join(", ", messageLines)}  (ErrorCode: {errorCode})");
  }
  
}
