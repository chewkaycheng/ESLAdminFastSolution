using Microsoft.Extensions.Logging;

namespace ESLAdmin.Logging.Interface
{
  public interface IMessageLogger
  {
    public ILogger Logger { get; }

    public void LogController(
      string funcName);

    public void LogController<T>(
    string funcName,
    T dto);

    public void LogControllerException(
      string funcName,
      Exception ex);

    public void LogSqlQuery(
    string funcName,
    string sql,
    string? parameters);

    public void LogSerializationFailure(
      string funcName,
      Exception ex);

    public void LogDatabaseExec(
      string funcName,
      string sql,
      string? parameters
     );

    public void LogDatabaseExecSuccess(
      string funcName,
      string sql,
      string? parameters
     );

    public void LogDatabaseExecFailure(
      string funcName,
      string sql,
      string? parameters);

    public void LogDatabaseException(
      string funcName,
      string sql,
      string? parameters,
      Exception ex);

    public void LogDatabaseException(
      string funcName,
      Exception ex);

  }
}
