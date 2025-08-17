using ErrorOr;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Common.Configuration;

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
  ErrorOr<Success> ValidateConfiguration(
    ILogger logger);
}
