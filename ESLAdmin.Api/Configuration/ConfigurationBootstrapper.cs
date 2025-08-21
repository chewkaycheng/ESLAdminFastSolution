using ESLAdmin.Common.Configuration;
using ESLAdmin.Logging;

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
  public ApiSettings Configure()
  {
    var apiSettings = _configuration.Get<ApiSettings>();
    if (apiSettings == null) {
      _tempLogger.LogError(
        "Failed to bind ApiSettings from configuration.");
      throw new InvalidOperationException("Failed to bind ApiSettings from configuration.");
    }

    var validator = new ApiSettingsValidator();
    var validationResult = validator.Validate(null, apiSettings);
    if (validationResult.Failed)
    {
      _tempLogger.LogError(
        $"Configuration validation failed: {validationResult.FailureMessage}");
      throw new InvalidOperationException($"ApiSettings validation failed: {validationResult.FailureMessage}");
    }

    return apiSettings;
  }
}
