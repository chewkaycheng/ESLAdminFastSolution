using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using ErrorOr;
using System.Text;

namespace ESLAdmin.Logging.Extensions;

public static class ValidationFailuresExtension
{
  public static string ToFormattedString(this List<ValidationFailure> failures)
  {
    StringBuilder sb = new StringBuilder();
    sb.Append("\n  Validation Failures:");
    sb.Append(string.Join("", failures.Select(f => $"\n   Code: {f.PropertyName} Description: {f.ErrorMessage}")));
    return sb.ToString();
  }
}

public static class IdentityResultExtensions
{
  public static string ToFormattedString(this IEnumerable<IdentityError> errors)
  {
    StringBuilder sb = new StringBuilder();
    sb.Append("\n  Identity Errors:");
    sb.Append(string.Join("", errors.Select(e => $"\n   Code: {e.Code} Description: {e.Description}")));
    return sb.ToString();
  }
}

public static class ErrorExtensions
  {
  public static string ToFormattedString(this IEnumerable<Error> errors)
  {
    StringBuilder sb = new StringBuilder();
    sb.Append("\n  ErrorOr Errors");
    foreach (var error in errors)
    {
      sb.Append($"\n   Code: {error.Code}");
      sb.Append($"\n   Description: {error.Description}");
      if (error.Metadata != null && error.Metadata.Any())
      {
        sb.Append("\n   Metadata:");
        foreach (var kvp in error.Metadata)
        {
          sb.Append($"\n     {kvp.Key}: {kvp.Value}");
        }
      }
    }
    return sb.ToString();
  }
}