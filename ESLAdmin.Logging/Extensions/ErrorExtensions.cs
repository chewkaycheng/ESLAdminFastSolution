using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;
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