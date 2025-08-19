using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace ESLAdmin.Common.Configuration;



//------------------------------------------------------------------------------
//
//                        class ConfigurationParams
//
//-------------------------------------------------------------------------------
public class ConfigurationParams : IConfigurationParams
{

  public class ConnectionStringsClass
  {
    public string ESLAdminConnection { get; set; }  
  }

  public class JwtSettingsClass
  {
    public class TokenValidationSettings
    {
      public bool ValidateIssuer { get; init; }
      public bool ValidateAudience { get; init; }
      public bool ValidateLifetime { get; init; }
      public bool ValidateIssuerSigningKey { get; init; }
    }

    public string Key { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public TokenValidationSettings TokenValidation { get; set; } = new TokenValidationSettings();
  }

  public class IdentitySettingsClass
  {
    public class UserSettings
    {
      public bool RequireUniqueEmail {  get; init; }
    }
    public class PasswordSettings
    {
      public int RequiredLength { get; init; }
      public bool RequireDigit { get; init; }
      public bool RequireLowercase { get; init; }
      public bool RequireUppercase { get; init; } 
      public bool RequireNonAlphanumeric { get; init; }
    }

    public class LockoutSettings
    {
      public string DefaultLockoutTimeSpan {  get; init; }
      public int MaxFailedAccessAttempts { get; init; }
    }

    public class SignInSettings
    {
      public bool RequireConfirmedEmail { get; init; }
      public bool RequireConfirmedPhoneNumber { get; init; }
    }

    public UserSettings User { get; set; } = new UserSettings();
    public PasswordSettings Password { get; init; } = new PasswordSettings();
    public LockoutSettings Lockout { get; init; } = new LockoutSettings();
    public SignInSettings SignIn { get; init; } = new SignInSettings();
    
  }

  const string JWT_SECTION = "Jwt";
  const string CONNECTION_STRINGS_SECTION = "ConnectionStrings";
  const string IDENTITY_SECTION = "Identity";
  

  const string JWT_KEY = "Jwt:Key";
  const string JWT_SECRET = "Jwt:Secret";
  const string JWT_AUDIENCE = "Jwt:Audience";
  const string JWT_ISSUER = "Jwt:Issuer";

  const string TOKEN_VALIDATION_VALIDATE_ISSUER = "Jwt:TokenValidation:ValidateIssuer";
  const string TOKEN_VALIDATION_VALIDATE_AUDIENCE = "Jwt:TokenValidation:ValidateAudience";
  const string TOKEN_VALIDATION_VALIDATE_LIFETIME = "Jwt:TokenValidation:ValidateLifetime";
  const string TOKEN_VALIDATION_VALIDATE_ISSUER_SIGNING_KEY = 
    "Jwt:TokenValidation:ValidateIssuerSigningKey";


  const string ESLADMIN_CONNECTION = "ConnectionStrings:ESLAdminConnection";

  const string PWD_REQUIRED_LENGTH = "Identity:Password:RequiredLength";
  const string PWD_REQUIRE_DIGIT = "Identity:Password:RequireDigit";
  const string PWD_REQUIRE_LOWERCASE = "Identity:Password:RequireLowercase";
  const string PWD_REQUIRE_UPPERCASE = "Identity:Password:RequireUppercase";
  const string PWD_REQUIRE_NONALPHANUMERIC = "Identity:Password:RequireNonAlphanumeric";

  const string LOCKOUT_DEFAULT_LOCKOUT_TIMESPAN = "Identity:Lockout:DefaultLockoutTimeSpan";
  const string LOCKOUT_MAX_FAILED_ACCESS_ATTEMPTS = "Identity:Lockout:MaxFailedAccessAttempts";

  const string USER_REQUIRE_UNIQUE_EMAIL = "Identity:User:RequireUniqueEmail";

  const string SIGNIN_REQUIRE_CONFIRMED_EMAIL = "Identity:SignIn:RequireConfirmedEmail";
  const string SIGNIN_REQUIRE_CONFIRMED_PHONENUMBER = "Identity:SignIn:RequireConfirmedPhoneNumber";


  private readonly IdentitySettingsClass _identitySettings;
  private readonly JwtSettingsClass _jwtSettings;
  private readonly ConnectionStringsClass _connectionStringsSettings;


  public IdentitySettingsClass IdentitySettings => _identitySettings;
  public JwtSettingsClass JwtSettings => _jwtSettings;
  public ConnectionStringsClass ConnectionStringsSettings => _connectionStringsSettings;

  private readonly IConfiguration _config;

  //------------------------------------------------------------------------------
  //
  //                        ConfigurationParams
  //
  //-------------------------------------------------------------------------------
  public ConfigurationParams(IConfiguration config)
  {
    _config = config;
    _jwtSettings = config
      .GetSection(JWT_SECTION)
      .Get<JwtSettingsClass>() ?? new JwtSettingsClass();
    _identitySettings = config
      .GetSection(IDENTITY_SECTION)
      .Get<IdentitySettingsClass>() ?? new IdentitySettingsClass();
    _connectionStringsSettings = config
      .GetSection(CONNECTION_STRINGS_SECTION)
      .Get<ConnectionStringsClass>() ?? new ConnectionStringsClass();
  }

  //------------------------------------------------------------------------------
  //
  //                        ValidateConfiguration
  //
  //-------------------------------------------------------------------------------
  public ErrorOr<Success> ValidateConfiguration(ILogger logger)
  {
    var errors = new List<Error>();

    // Validate Jwt settings
    if (string.IsNullOrEmpty(_jwtSettings.Key))
      errors.Add(AppErrors.CommonErrors.ConfigurationMissingKey(JWT_KEY));
    //if (string.IsNullOrEmpty(_jwtSettings.Secret))
    //  errors.Add(AppErrors.CommonErrors.ConfigurationMissingKey(JWT_SECRET));
    if (string.IsNullOrEmpty(_jwtSettings.Audience))
      errors.Add(AppErrors.CommonErrors.ConfigurationMissingKey(JWT_AUDIENCE));
    if (string.IsNullOrEmpty(_config[JWT_ISSUER]))
      errors.Add(AppErrors.CommonErrors.ConfigurationMissingKey(JWT_ISSUER));

    // Validate Jwt:TokenValidation settings
    if (string.IsNullOrEmpty(_config[TOKEN_VALIDATION_VALIDATE_ISSUER]) ||
    !bool.TryParse(_config[TOKEN_VALIDATION_VALIDATE_ISSUER], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          TOKEN_VALIDATION_VALIDATE_ISSUER, "Must be 'true' or 'false'."));
    if (string.IsNullOrEmpty(_config[TOKEN_VALIDATION_VALIDATE_AUDIENCE]) ||
    !bool.TryParse(_config[TOKEN_VALIDATION_VALIDATE_AUDIENCE], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          TOKEN_VALIDATION_VALIDATE_AUDIENCE, "Must be 'true' or 'false'."));
    if (string.IsNullOrEmpty(_config[TOKEN_VALIDATION_VALIDATE_LIFETIME]) ||
    !bool.TryParse(_config[TOKEN_VALIDATION_VALIDATE_LIFETIME], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          TOKEN_VALIDATION_VALIDATE_LIFETIME, "Must be 'true' or 'false'."));
    if (string.IsNullOrEmpty(_config[PWD_REQUIRE_NONALPHANUMERIC]) ||
    !bool.TryParse(_config[PWD_REQUIRE_NONALPHANUMERIC], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          PWD_REQUIRE_NONALPHANUMERIC, "Must be 'true' or 'false'."));


    // Validate connection string
    if (string.IsNullOrEmpty(_connectionStringsSettings.ESLAdminConnection))
      errors.Add(AppErrors.CommonErrors.ConfigurationMissingKey(ESLADMIN_CONNECTION));

    // Validate Identity:Password settings
    if (_identitySettings.Password.RequiredLength < 5)
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          PWD_REQUIRED_LENGTH, "Must be an integer >= 5."));

    // Validate boolean settings with direct config checks for consistency
    if (string.IsNullOrEmpty(_config[PWD_REQUIRE_DIGIT]) ||
        !bool.TryParse(_config[PWD_REQUIRE_DIGIT], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          PWD_REQUIRE_DIGIT, "Must be 'true' or 'false'."));
    if (string.IsNullOrEmpty(_config[PWD_REQUIRE_LOWERCASE]) ||
        !bool.TryParse(_config[PWD_REQUIRE_LOWERCASE], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          PWD_REQUIRE_LOWERCASE, "Must be 'true' or 'false'."));
    if (string.IsNullOrEmpty(_config[PWD_REQUIRE_UPPERCASE]) ||
        !bool.TryParse(_config[PWD_REQUIRE_UPPERCASE], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          PWD_REQUIRE_UPPERCASE, "Must be 'true' or 'false'."));
    if (string.IsNullOrEmpty(_config[PWD_REQUIRE_NONALPHANUMERIC]) ||
        !bool.TryParse(_config[PWD_REQUIRE_NONALPHANUMERIC], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          PWD_REQUIRE_NONALPHANUMERIC, "Must be 'true' or 'false'."));

    // Validate Identity:Lockout settings
    if (string.IsNullOrEmpty(_identitySettings.Lockout.DefaultLockoutTimeSpan) ||
        !TimeSpan.TryParse(_identitySettings.Lockout.DefaultLockoutTimeSpan, out var lockoutTimeSpan) ||
        lockoutTimeSpan <= TimeSpan.Zero)
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          LOCKOUT_DEFAULT_LOCKOUT_TIMESPAN, "Must be a valid TimeSpan > 00:00:00."));
    if (_identitySettings.Lockout.MaxFailedAccessAttempts < 1)
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          LOCKOUT_MAX_FAILED_ACCESS_ATTEMPTS, "Must be an integer >= 1."));

    // Validate Identity:User settings
    if (string.IsNullOrEmpty(_config[USER_REQUIRE_UNIQUE_EMAIL]) ||
    !bool.TryParse(_config[USER_REQUIRE_UNIQUE_EMAIL], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          USER_REQUIRE_UNIQUE_EMAIL, "Must be 'true' or 'false'."));

    // Validate Identity:SignIn settings
    if (string.IsNullOrEmpty(_config[SIGNIN_REQUIRE_CONFIRMED_EMAIL]) ||
        !bool.TryParse(_config[SIGNIN_REQUIRE_CONFIRMED_EMAIL], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          SIGNIN_REQUIRE_CONFIRMED_EMAIL, "Must be 'true' or 'false'."));
    if (string.IsNullOrEmpty(_config[SIGNIN_REQUIRE_CONFIRMED_PHONENUMBER]) ||
        !bool.TryParse(_config[SIGNIN_REQUIRE_CONFIRMED_PHONENUMBER], out _))
      errors.Add(AppErrors.CommonErrors.ConfigurationInvalidValue(
          SIGNIN_REQUIRE_CONFIRMED_PHONENUMBER, "Must be 'true' or 'false'."));

    if (errors.Any())
    {
      logger.LogError("Configuration validation failed: {Errors}", string.Join(", ", errors.Select(e => e.Description)));
      return errors;
    }

    logger.LogInformation("Configuration validated successfully.");
    return Result.Success;
  }
}
