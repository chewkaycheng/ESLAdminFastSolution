using ESLAdmin.Common.Configuration;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Extensions;

namespace ESLAdmin.Api.Configuration;

//------------------------------------------------------------------------------
//
//                        class ConfigurationBootstrapper
//
//-------------------------------------------------------------------------------
public class ConfigurationBootstrapper
{
  private readonly IConfiguration _configuration;
  private readonly IServiceCollection _services;
  private readonly ILogger<ConfigurationBootstrapper> _tempLogger;

  //------------------------------------------------------------------------------
  //
  //                        ConfigurationBootstrapper
  //
  //-------------------------------------------------------------------------------
  public ConfigurationBootstrapper(
    IServiceCollection services,
    IConfiguration configuration)
  {
    _configuration = configuration;
    _services = services;
    _tempLogger = CreateTempLogger();
  }

  public ILogger<ConfigurationBootstrapper> CreateTempLogger()
  {
    var loggerFactory = LoggerFactory.Create(builder =>
    {
      builder.AddProvider(new SimpleFileLoggerProvider());
    });
    return loggerFactory.CreateLogger<ConfigurationBootstrapper>();
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //-------------------------------------------------------------------------------
  public IConfigurationParams Configure()
  {
    // Create abd reguster IConfigurationParams
    var configParams = new ConfigurationParams(_configuration);
    
    // Validate the configuration
    var result = configParams.ValidateConfiguration(_tempLogger);
    if (result.IsError)
    {
      _tempLogger.LogError(
        $"Configuration validation failed: {result.Errors.ToFormattedString()}");
      throw new InvalidOperationException(
          $"Configuration validation failed: {string.Join("; ", result.Errors.Select(e => e.Description))}");
    }
    _services.AddSingleton<IConfigurationParams>(configParams);

    return configParams;
  }
}
