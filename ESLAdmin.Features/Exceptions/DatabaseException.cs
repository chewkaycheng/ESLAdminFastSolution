namespace ESLAdmin.Features.Exceptions;

// =================================================
// 
// DatabaseException
//
// ==================================================
public class DatabaseException : Exception
{
  public DatabaseException(
    string funcName,
    string sql,
    string? parameters,
    Exception? innerException
    )
  : base(
      innerException == null ?
        string.Format("A Database exception has occurred. \nFunction: {0} \nSql: {1} \nParams: {2}",
          funcName,
          sql,
          parameters) :
        string.Format("A Database exception has occurred. \nFunction: {0} \nSql: {1} \nParams: {2} \nInnerException: {3}",
          funcName,
          sql,
          parameters,
          innerException))
  {
  }

  public DatabaseException(
    string funcName,
    Exception? innerException)
    : base(string.Format("A Database exception has occurred. \nFunction: {0}",
          funcName), innerException)
  {
  }

}
