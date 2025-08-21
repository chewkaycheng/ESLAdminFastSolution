using Microsoft.Extensions.Options;
using MiniValidation;
using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Common.Configuration;

public class ApiSettingsValidator : IValidateOptions<ApiSettings>
{
  public ValidateOptionsResult Validate(
    string? name, 
    ApiSettings options)
  {
    var context = new ValidationContext(options);
    var results = new List<ValidationResult>();

    if (!MiniValidator.TryValidate(options, out var validationErrors))
    {
      string typeName = options.GetType().Name;
      foreach(var (member, memberErrors) in validationErrors)
      {
        results.Add(new ValidationResult(
          $"Data annotation failed for member: '{member}' with errors: '{string.Join("', '", memberErrors)}"));
      }
    }

    // Custom validation for DefaultLockoutTimeSpan
    if (!string.IsNullOrEmpty(
         options.Identity.Lockout.DefaultLockoutTimeSpan))
    {
      if (!TimeSpan.TryParse(
            options.Identity.Lockout.DefaultLockoutTimeSpan, 
            out var lockoutTimeSpan) ||
            lockoutTimeSpan <= TimeSpan.Zero)
      {
        results.Add(new ValidationResult(
          "Lockout:DefaultLockoutTimeSpan must be a valid TimeSpan > 00:00:00",
          new[] { nameof(options.Identity.Lockout.DefaultLockoutTimeSpan) }));
      }
    }

    if (results.Any())
    {
      var errors = "\n  " + string.Join("\n  ", results.Select(r => r.ErrorMessage));
      return ValidateOptionsResult.Fail(errors);
    }

    return ValidateOptionsResult.Success;
  }
}
