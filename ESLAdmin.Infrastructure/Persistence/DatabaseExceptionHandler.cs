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
      FbException fbe => AppErrors.DatabaseExceptions.DatabaseException(
        $"Firebird error: {fbe.Message} (ErrorCode: {fbe.ErrorCode})"),
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
}
