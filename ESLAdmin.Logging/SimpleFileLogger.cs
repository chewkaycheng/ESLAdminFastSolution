using Microsoft.Extensions.Logging;

namespace ESLAdmin.Logging;

public class SimpleFileLogger : ILogger
{
  private readonly string _categoryName;
  private readonly string _logFilePath;

  public SimpleFileLogger(string categoryName)
  {
    _categoryName = categoryName;
    
    var logFileDirectory = Path.Combine(
      Directory.GetCurrentDirectory(), "Logs");
    Directory.CreateDirectory(logFileDirectory);

    _logFilePath = Path.Combine(
      Directory.GetCurrentDirectory(), $"Logs\\initial-config-validation-{DateTime.Now:yyyy-MM-dd}.log");
  }

  public IDisposable BeginScope<TState>(TState state) => null;

  public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;

  public void Log<TState>(
    LogLevel logLevel, 
    EventId eventId, 
    TState state, 
    Exception exception, 
    Func<TState, Exception, string> formatter)
  {
    if (!IsEnabled(logLevel)) return;

    var message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {formatter(state, exception)}";
    File.AppendAllText(_logFilePath, message + Environment.NewLine);
  }
}

public class SimpleFileLoggerProvider : ILoggerProvider
{
  public ILogger CreateLogger(string categoryName) => new SimpleFileLogger(categoryName);
  public void Dispose() { }
}
