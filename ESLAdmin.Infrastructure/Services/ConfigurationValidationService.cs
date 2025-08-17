using ESLAdmin.Infrastructure.Configuration;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Services;

public class ConfigurationValidationService : IHostedService
{
  private readonly IConfigurationParams _configParams;
  private readonly ILogger<ConfigurationValidationService> _logger;

  public ConfigurationValidationService(
    IConfigurationParams configParams, 
    ILogger<ConfigurationValidationService> logger)
  {
    _configParams = configParams;
    _logger = logger;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogFunctionEntry("Starting configuration validation...");
    var result = _configParams.ValidateConfiguration(_logger);
    if (result.IsError)
    {
      _logger.LogError($"Configuration validation failed: {result.Errors.ToFormattedString()}");
      throw new InvalidOperationException(
        $"Configuration validation failed: {string.Join("; ", result.Errors.Select(e => e.Description))}");
    }

    _logger.LogFunctionExit("Configuration validation completed successfully.");
    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
