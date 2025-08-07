using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Common.Errors;

public static partial class Errors
{
  //-------------------------------------------------------------------------------
  //
  //                       AddMetadata
  //
  //-------------------------------------------------------------------------------
  private static void AddMetadata(
    Dictionary<string, object>? metadata,
    IEnumerable<IdentityError> errors)
  {
    if (errors != null && errors.Any())
    {
      foreach (var err in errors)
      {
        metadata?.Add(err.Code, err.Description);
      }
    }
  }
}
