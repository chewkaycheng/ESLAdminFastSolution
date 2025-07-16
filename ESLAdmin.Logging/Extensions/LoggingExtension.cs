using ESLAdmin.Logging.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO.Compression;

namespace ESLAdmin.Logging.Extensions;

public static class LoggingExtension
{
  public static void ConfigureLogging(
    this IServiceCollection services,
    IHostApplicationBuilder builder)
  {
    var _logDirectory = Path.Combine(
      Directory.GetCurrentDirectory(), "Logs");
    var _zipDirectory = Path.Combine(
      Path.Combine(_logDirectory, "zipped"));

    Directory.CreateDirectory(_logDirectory);
    Directory.CreateDirectory(_zipDirectory);

    Log.Logger = new LoggerConfiguration()
      .WriteTo.File(
        path: "",
        hooks: new ZipFileHook(_zipDirectory))
      .ReadFrom.Configuration(builder.Configuration)
      .CreateLogger();

    services.AddSerilog(Log.Logger, dispose: true);

    services.AddSingleton<IMessageLogger, MessageLogger>();
  }
}

public class ZipFileHook : Serilog.Sinks.File.FileLifecycleHooks
{
  private readonly string _zipDirectory;

  public ZipFileHook(string zipDirectory)
  {
    _zipDirectory = zipDirectory;
  }

  public override void OnFileDeleting(string path)
  {
    try
    {
      var date = Path.GetFileNameWithoutExtension(path).Split('-').Last();
      date = date.Split('_').First();
      var zipFileName = Path.Combine(_zipDirectory, $"log-{date}.zip");
      var fileName = Path.GetFileName(path);

      lock (this)
      {
        using (var zip = File.Exists(zipFileName) ?
               ZipFile.Open(zipFileName, ZipArchiveMode.Update) :
               ZipFile.Open(zipFileName, ZipArchiveMode.Create))
        {
          zip.CreateEntryFromFile(path, fileName, CompressionLevel.Optimal);
        }
      }
    }
    catch (Exception ex)
    {
      // Log error to console to avoid recursive logging
      Console.WriteLine($"Error zipping file {path}: {ex.Message}");
    }
    base.OnFileDeleting(path);
  }
}
