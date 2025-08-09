using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Configuration;

//------------------------------------------------------------------------------
//
//                        interface IConfigurationParams
//
//-------------------------------------------------------------------------------
public interface IConfigurationParams
{

  IReadOnlyDictionary<string, string> Settings { get; }

  //------------------------------------------------------------------------------
  //
  //                        ValidateConfiguration
  //
  //-------------------------------------------------------------------------------
  void ValidateConfiguration(
    ILogger logger);
}
