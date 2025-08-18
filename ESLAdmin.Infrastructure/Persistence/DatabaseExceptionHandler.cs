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
      ArgumentNullException ane => AppErrors.IdentityErrors.InvalidArgument(ane.Message),
      DbUpdateException dbu => AppErrors.DatabaseErrors.DatabaseError(
        ex.InnerException?.Message ?? ex.Message),
      FbException fbe => AppErrors.DatabaseErrors.DatabaseError(
        $"Firebird error: {fbe.Message} (ErrorCode: {fbe.ErrorCode})"),
      InvalidOperationException ioe => AppErrors.IdentityErrors.InvalidOperation(ioe.Message),
      OperationCanceledException => AppErrors.DatabaseErrors.OperationCanceled(),
      _ => AppErrors.CommonErrors.Exception(ex.Message)

    };
  }
}
