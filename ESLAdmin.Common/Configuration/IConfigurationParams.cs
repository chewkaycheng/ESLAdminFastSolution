using ErrorOr;
using Microsoft.Extensions.Logging;
using static ESLAdmin.Common.Configuration.ConfigurationParams;

namespace ESLAdmin.Common.Configuration;

//------------------------------------------------------------------------------
//
//                        interface IConfigurationParams
//
//-------------------------------------------------------------------------------
public interface IConfigurationParams
{
  JwtSettingsClass JwtSettings { get; }
  IdentitySettingsClass IdentitySettings { get; }
  ConnectionStringsClass ConnectionStringsSettings { get; }

  //------------------------------------------------------------------------------
  //
  //                        ValidateConfiguration
  //
  //-------------------------------------------------------------------------------
  ErrorOr<Success> ValidateConfiguration(
    ILogger logger);
}
