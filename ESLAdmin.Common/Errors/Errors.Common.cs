using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Common.Errors;

//-------------------------------------------------------------------------------
//
//                       partial class Errors
//
//-------------------------------------------------------------------------------
public static partial class Errors
{
  //-------------------------------------------------------------------------------
  //
  //                       class CommonErrors
  //
  //-------------------------------------------------------------------------------\
  public static class CommonErrors
  {
    //-------------------------------------------------------------------------------
    //
    //                       Exception
    //
    //-------------------------------------------------------------------------------\
    public static Error Exception(string message) =>
      Error.Failure(
        code: "Exception",
        description: message);

    //-------------------------------------------------------------------------------
    //
    //                       ConfigurationError
    //
    //-------------------------------------------------------------------------------
    public static Error ConfigurationError(IEnumerable<IdentityError>? errors = null)
    {
      var error = Error.Failure(
        code: "ConfigurationError",
        description: $"Parameter(s) not found in configuration file.");

      if (errors != null && errors.Any())
      {
        AddMetadata(error.Metadata, errors);
      }
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       ConfigurationMissingKey
    //
    //-------------------------------------------------------------------------------
    public static Error ConfigurationMissingKey(string key) =>
          Error.Failure(
            "Configuration.MissingKey",
            $"Configuration key '{key}' is not found in the configuration file");

    //-------------------------------------------------------------------------------
    //
    //                       ConfigurationError
    //
    //-------------------------------------------------------------------------------
    public static Error ConfigurationError(IEnumerable<Error> errors) =>
          Error.Failure(
            "Configuration.Error",
            string.Join("; ", errors.Select(e => e.Description)));
  }
}
