using ErrorOr;
using ESLAdmin.Common.Errors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Configuration;

//------------------------------------------------------------------------------
//
//                        class ConfigurationParams
//
//-------------------------------------------------------------------------------
public class ConfigurationParams : IConfigurationParams
{
  private readonly Dictionary<string, string> _settings;
  public IReadOnlyDictionary<string, string> Settings => _settings;

  //------------------------------------------------------------------------------
  //
  //                        ConfigurationParams
  //
  //-------------------------------------------------------------------------------
  public ConfigurationParams(IConfiguration config)
  {
    _settings = new Dictionary<string, string>
    {
      { "Jwt:Key", config["Jwt:Key"] ?? "" },
      { "Jwt:Issuer", config["Jwt:Issuer"] ?? "" },
      { "Jwt:Audience", config["Jwt:Audience"] ?? "" },
      { "ConnectionStrings:ESLAdminConnection", config["ConnectionStrings:ESLAdminConnection"] ?? "" }
    };
  }

  //------------------------------------------------------------------------------
  //
  //                        ValidateConfiguration
  //
  //-------------------------------------------------------------------------------
  public ErrorOr<Success> ValidateConfiguration(ILogger logger)
  {
    var missingKeys = _settings
     .Where(kvp => string.IsNullOrEmpty(kvp.Value))
     .Select(kvp => kvp.Key)
     .ToList();

    if (missingKeys.Any())
    {
      logger.LogError("Missing or empty configuration keys: {MissingKeys}", string.Join(", ", missingKeys));
      return missingKeys.Select(
        key => Errors.CommonErrors.ConfigurationMissingKey(key)).ToList();
    }

    return Result.Success;
  }
}
