using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Common.CustomErrors;

//-------------------------------------------------------------------------------
//
//                       partial class Errors
//
//
//-------------------------------------------------------------------------------
public static partial class AppErrors
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

    //-------------------------------------------------------------------------------
    //
    //                       ConfigurationInvalidValue
    //
    //-------------------------------------------------------------------------------
    public static Error ConfigurationInvalidValue(string key, string description) =>
        Error.Failure(
          $"Configuration.Invalid.{key}", 
          $"Invalid configuration for '{key}': {description}");
  }
}
