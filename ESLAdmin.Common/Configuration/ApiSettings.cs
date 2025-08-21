using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Common.Configuration;

public class ApiSettings
{
  public class ConnectionStringsSettings
  {
    [Required(ErrorMessage = "ESLAdminConnection is required.")]
    [MinLength(1)]
    public string ESLAdminConnection { get; set; } = null!;
  }

  public class JwtSettings
  {
    public class TokenValidationSettings
    {
      [Required(ErrorMessage = "ValidateIssuer is required.")]
      public bool? ValidateIssuer { get; init; } = null!;

      [Required(ErrorMessage = "ValidateAudience is required.")]
      public bool? ValidateAudience { get; init; } = null!;

      [Required(ErrorMessage = "ValidateLifetime is required.")]
      public bool? ValidateLifetime { get; init; } = null!;

      [Required(ErrorMessage = "ValidateIssuerSigningKey is required.")]
      public bool? ValidateIssuerSigningKey { get; init; } = null!;
    }

    [Required(ErrorMessage = "Jwt:Key is required.")]
    public string Key { get; init; } = null!;

    [Required(ErrorMessage = "Jwt:Issuer is required.")]
    public string Issuer { get; init; } = null!;

    [Required(ErrorMessage = "Jwt:Audience is required.")]
    public string Audience { get; init; } = null!;

    [Required(ErrorMessage = "Jwt:TokenValidation is required.")]
    public TokenValidationSettings TokenValidation { get; init; } = new TokenValidationSettings();
  }
  public class IdentitySettings
  {
    public class UserSettings
    {
      [Required(ErrorMessage = "User:RequireUniqueEmail is required.")]
      public bool? RequireUniqueEmail { get; init; } = null!;
    }

    public class PasswordSettings
    {
      [Range(5, int.MaxValue, ErrorMessage = "Password:RequiredLength must be at least 5.")]
      public int RequiredLength { get; init; }

      [Required(ErrorMessage = "Password:RequireDigit is required.")]
      public bool RequireDigit { get; init; }

      [Required(ErrorMessage = "Password:RequireLowercase is required.")]
      public bool RequireLowercase { get; init; }

      [Required(ErrorMessage = "Password:RequireUppercase is required.")]
      public bool RequireUppercase { get; init; }

      [Required(ErrorMessage = "Password:RequireNonAlphanumeric is required.")]
      public bool RequireNonAlphanumeric { get; init; }
    }

    public class LockoutSettings
    {
      [Required(ErrorMessage = "Lockout:DefaultLockoutTimeSpan is required.")]
      public string DefaultLockoutTimeSpan { get; init; } = string.Empty;

      [Range(1, int.MaxValue, ErrorMessage = "Lockout:MaxFailedAccessAttempts must be at least 1.")]
      public int MaxFailedAccessAttempts { get; init; }
    }

    public class SignInSettings
    {
      [Required(ErrorMessage = "SignIn:RequireConfirmedEmail is required.")]
      public bool RequireConfirmedEmail { get; init; }

      [Required(ErrorMessage = "SignIn:RequireConfirmedPhoneNumber is required.")]
      public bool RequireConfirmedPhoneNumber { get; init; }
    }

    [Required(ErrorMessage = "Identity:User is required.")]
    public UserSettings User { get; init; } = new UserSettings();

    [Required(ErrorMessage = "Identity:Password is required.")]
    public PasswordSettings Password { get; init; } = new PasswordSettings();

    [Required(ErrorMessage = "Identity:Lockout is required.")]
    public LockoutSettings Lockout { get; init; } = new LockoutSettings();

    [Required(ErrorMessage = "Identity:SignIn is required.")]
    public SignInSettings SignIn { get; init; } = new SignInSettings();
  }

  [Required(ErrorMessage = "ConnectionStrings is required.")]
  public ConnectionStringsSettings ConnectionStrings { get; init; } = new ConnectionStringsSettings();

  [Required(ErrorMessage = "Jwt is required.")]
  public JwtSettings Jwt { get; init; } = new JwtSettings();

  [Required(ErrorMessage = "Identity is required.")]
  public IdentitySettings Identity { get; init; } = new IdentitySettings();
}
