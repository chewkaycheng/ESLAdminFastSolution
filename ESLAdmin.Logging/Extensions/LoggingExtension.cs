using ESLAdmin.Logging.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Templates;
using System.IO.Compression;

namespace ESLAdmin.Logging.Extensions;

//------------------------------------------------------------------------------
//
//                      static class LoggingExtension
//
//------------------------------------------------------------------------------
public static class LoggingExtension
{
  //------------------------------------------------------------------------------
  //
  //                     ConfigureLogging
  //
  //------------------------------------------------------------------------------
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
      .MinimumLevel.Debug()
      .WriteTo.File(
        formatter: new ExpressionTemplate(
                   "[{@t:dd/MM/yyyy HH:mm:ss} [{@l:u3}] {SourceContext}] {@m}\n{@x}"),
        path: Path.Combine(_logDirectory, "log-.log"),
        hooks: new ZipFileHook(_zipDirectory),
        rollingInterval: Serilog.RollingInterval.Day,
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 2097152,
        retainedFileCountLimit: 1,
        buffered: false
       )
      .ReadFrom.Configuration(builder.Configuration)
      //.WriteTo.File(
      //  new ExpressionTemplate(
      //    "[{@t:dd/mm/yyyy HH:mm:ss} {@l:u3} {SourceContext}] {@m}\n{@x}"),
      //  path: "/logs/log-.txt",
      //  hooks: new ZipFileHook(_zipDirectory))
      //.ReadFrom.Configuration(builder.Configuration)
      .CreateLogger();

    services.AddSerilog(Log.Logger, dispose: true);

    services.AddSingleton<IMessageLogger, MessageLogger>();
  }
}

//------------------------------------------------------------------------------
//
//                     class ZipFileHook
//
//------------------------------------------------------------------------------
public class ZipFileHook : Serilog.Sinks.File.FileLifecycleHooks
{
  private readonly string _zipDirectory;

  //------------------------------------------------------------------------------
  //
  //                     ZipFileHook
  //
  //------------------------------------------------------------------------------
  public ZipFileHook(string zipDirectory)
  {
    _zipDirectory = zipDirectory;
  }

  //------------------------------------------------------------------------------
  //
  //                     OnFileDeleting
  //
  //------------------------------------------------------------------------------
  public override void OnFileDeleting(string path)
  {
    try
    {
      var dateStr = Path.GetFileNameWithoutExtension(path).Split('-').Last();
      dateStr = dateStr.Split('_').First();
      var zipFileName = Path.Combine(_zipDirectory, $"log-{dateStr}.zip");
      var fileName = Path.GetFileName(path);

      lock (this)
      {
        using var zip = File.Exists(zipFileName) ?
          ZipFile.Open(zipFileName, ZipArchiveMode.Update) :
          ZipFile.Open(zipFileName, ZipArchiveMode.Create);
        zip.CreateEntryFromFile(path, fileName, CompressionLevel.Optimal);
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
