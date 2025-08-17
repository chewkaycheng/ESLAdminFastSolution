using ESLAdmin.Logging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Configuration;

//------------------------------------------------------------------------------
//
//                        class ConfigurationBootstrapper
//
//-------------------------------------------------------------------------------
public class ConfigurationBootstrapper
{
  private readonly IConfiguration _configuration;
  private readonly IServiceCollection _services;
  private readonly ILoggerFactory _loggerFactory;

  //------------------------------------------------------------------------------
  //
  //                        ConfigurationBootstrapper
  //
  //-------------------------------------------------------------------------------
  public ConfigurationBootstrapper(
    IServiceCollection services,
    IConfiguration configuration,
    ILoggerFactory loggerFactory)
  {
    _configuration = configuration;
    _services = services;
    _loggerFactory = loggerFactory;
  }

  //------------------------------------------------------------------------------
  //
  //                        Configure
  //
  //-------------------------------------------------------------------------------
  public IConfigurationParams Configure()
  {
    // Create a logger
    var logger = _loggerFactory.CreateLogger<ConfigurationBootstrapper>();

    // Create abd reguster IConfigurationParams
    var configParams = new ConfigurationParams(_configuration);
    _services.AddSingleton<IConfigurationParams>(configParams);

    // Validate the configuration
    var result = configParams.ValidateConfiguration(logger);
    if (result.IsError)
    {
      logger.LogError(
        $"Configuration validation failed: {result.Errors.ToFormattedString()}");
      throw new InvalidOperationException(
          $"Configuration validation failed: {string.Join("; ", result.Errors.Select(e => e.Description))}");
    }

    return configParams;
  }
}
