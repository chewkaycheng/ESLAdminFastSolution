using ErrorOr;
using ESLAdmin.Common.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace ESLAdmin.Infrastructure.Utilities;

public static class ConfigurationValidator
{
  public static ErrorOr<Dictionary<string, string>> ValidateConfiguration(
    ILogger logger,
    IConfiguration configuration)
  {
    var configKeys = new Dictionary<string, string>
      {
        { "Jwt:Key", configuration["Jwt:Key"] ?? "" },
        { "Jwt:Issuer", configuration["Jwt:Issuer"] ?? "" },
        { "Jwt:Audience", configuration["Jwt:Audience"] ?? ""}
      };

    var missingKeys = configKeys
      .Where(kvp => string.IsNullOrEmpty(kvp.Value))
      .Select(kvp => kvp.Key)
      .ToList();

    if (missingKeys.Any())
    {
      var identityErrors = new List<IdentityError>();
      foreach (var configKey in missingKeys)
      {
        identityErrors.AddRange(new IdentityError
        {
          Code = $"{configKey}Missing",
          Description = $"{configKey} is not found in the configuration file."
        });
        logger.LogError("{ConfigKey} is not found in the configuration file", configKey);
      }
      return Errors.CommonErrors.ConfigurationError(identityErrors);
    }
    return configKeys;
  }
}
