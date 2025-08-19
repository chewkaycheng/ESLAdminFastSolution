using ErrorOr;
using ESLAdmin.Common.Configuration;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Common.CustomErrors;

//-------------------------------------------------------------------------------
//
//                       partial class Errors
//
//-------------------------------------------------------------------------------
public static partial class AppErrors
{
  private static IConfigurationParams? _configParams;

  public static void Initialize(IConfigurationParams configParams)
  {
    _configParams = configParams;
  }

  //-------------------------------------------------------------------------------
  //
  //                       class IdentityErrors
  //
  //-------------------------------------------------------------------------------
  public static class IdentityErrors
  {
    //-------------------------------------------------------------------------------
    //
    //                       TokenRevokactionFailed
    //
    //-------------------------------------------------------------------------------
    public static Error TokenRevocationFailed(
      string userId, 
      string description) =>
        Error.Failure(
          "Identity.TokenRevocationFailed", 
          $"Failed to revoke tokens for user '{userId}': {description}");

    //-------------------------------------------------------------------------------
    //
    //                       TokenGenerationFailed
    //
    //-------------------------------------------------------------------------------
    public static Error TokenGenerationFailed(
      string userId, 
      string description) =>
        Error.Failure(
          "Identity.TokenGenerationFailed", $"Failed to generate token for user '{userId}': {description}");

    //-------------------------------------------------------------------------------
    //
    //                       RefreshTokenInvalid
    //
    //-------------------------------------------------------------------------------
    public static Error RefreshTokenInvalid(
      string description) =>
        Error.Failure($"Identity.InvalidRefreshToken", description);

    //-------------------------------------------------------------------------------
    //
    //                       OperationCanceled
    //
    //-------------------------------------------------------------------------------
    public static Error OperationCanceled() =>
       Error.Failure(
         "Identity.OperationCanceled", 
         "Operation was canceled.");

    //-------------------------------------------------------------------------------
    //
    //                       RefreshTokenNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error RefreshTokenNotFound(string token) =>
      Error.Validation(
        code: "Identity.RefreshTokenNotFound",
        description: $"The refresh token: '{token}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidToken
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidToken() =>
      Error.Validation(
        code: "Identity.InvalidToken",
        description: "The token is invalid.");

    //-------------------------------------------------------------------------------
    //
    //                       AddToRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error AddToRoleFailed(string email, string roleName, IEnumerable<IdentityError>? errors = null)
    {
      var error = Error.Failure(
        code: "Identity.AddToRoleFailed",
        description: $"AddToRole failed for user: '{email}', role: '{roleName}'.");

      if (errors != null && errors.Any())
      {
        AddMetadata(error.Metadata, errors);
      }
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       InvalidRoles
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidRoles(List<string?> invalidRoles) =>
      Error.Validation(
        code: "Identity.InvalidRoles",
        description: $"The following roles do not exist: {string.Join(", ", invalidRoles)}");


    //-------------------------------------------------------------------------------
    //
    //                       UserEmailCannotBeEmpty
    //
    //-------------------------------------------------------------------------------
    public static Error UserEmailCannotBeEmpty() =>
      Error.Validation(
        code: "Identity.UserEmailEmpty",
        description: $"The email for the user cannot be empty.");

    //-------------------------------------------------------------------------------
    //
    //                       UserEmailNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error UserEmailNotFound(string email) =>
      Error.Validation(
        code: "Identity.NotFound",
        description: $"The user with email: '{email}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       UserIdNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error UserIdNotFound(string userId) =>
      Error.Validation(
        code: "Identity.NotFound",
        description: $"The user with Id: '{userId}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       UserNameNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error UserNameNotFound(string username) =>
      Error.Validation(
        code: "Identity.NotFound",
        description: $"The user with Username: '{username}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       UserLoginFailed
    //
    //-------------------------------------------------------------------------------
    public static Error UserLoginFailed() =>
      Error.Validation(
        code: "Identity.LoginFailed",
        description: $"Username or password is invalid.");

    //-------------------------------------------------------------------------------
    //
    //                       CreateUserFailed
    //
    //-------------------------------------------------------------------------------
    public static Error CreateUserFailed(string email, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure(
        code: "Identity.CreateUserFailed",
        description: $"Failed to create user: '{email}'.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       AddToRolesFailed
    //
    //-------------------------------------------------------------------------------
    public static Error AddToRolesFailed(string email, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure(
        code: "Identity.AddToRolesFailed",
        description: $"Failed to add roles for user: '{email}'.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       RemoveRolesFailed
    //
    //-------------------------------------------------------------------------------
    public static Error RemoveRolesFailed(string email, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure(
        code: "Identity.RemoveRolesFailed",
        description: $"Failed to remove roles from {email}.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       DeleteUserFailed
    //
    //-------------------------------------------------------------------------------
    public static Error DeleteUserFailed()
    {
      var error = Error.Failure(
        code: "Identity.DeleteUserFailed",
        description: "Failed to delete user.");

      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       AssignedRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error AssignRoleFailed(
      string email,
      string roleName,
      IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure(
        code: "Identity.AssignRoleFailed",
        description: $"Failed to assign role: '{roleName}' to user: '{email}'.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       UserAlreadyInRole
    //
    //-------------------------------------------------------------------------------
    public static Error UserAlreadyInRole(string email, string roleName) =>
      Error.Validation(
        code: "Identity.UserAlreadyInRole",
        description: $"The role: '{roleName}' is already assigned to the user: '{email}'.");

    //-------------------------------------------------------------------------------
    //
    //                       RoleNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error RoleNotFound(string roleName) =>
      Error.Validation(
        code: "Identity.RoleNotFound",
        description: $"The role: '{roleName}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       RoleExists
    //
    //-------------------------------------------------------------------------------
    public static Error RoleExists(string roleName) =>
      Error.Conflict(
        code: "Identity.RoleExists",
        description: $"The role: '{roleName}' already exists.");

    //-------------------------------------------------------------------------------
    //
    //                       CreateRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error CreateRoleFailed(string roleName, IEnumerable<IdentityError> errors)
    {

      var error = Error.Failure(
        code: "Identity.CreateRoleFailed",
        description: $"Failed to create role: '{roleName}'.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       UpdateRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error UpdateRoleFailed(string oldRoleName, string newRoleName, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure(
        code: "Identity.UpdateRoleFailed",
        description: $"Failed to update role: '{oldRoleName}' with '{newRoleName}'.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       DeleteRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error DeleteRoleFailed(string roleName, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure(
        code: "Identity.DeleteRoleFailed",
        description: $"Failed to delete role: '{roleName}'.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       InvalidUser
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidUser(string description) =>
        Error.Failure("Identity.InvalidUser", description);

    //-------------------------------------------------------------------------------
    //
    //                       InvalidRole
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidRole(string description) =>
        Error.Failure("Identity.InvalidRole", description);

    //-------------------------------------------------------------------------------
    //
    //                       UserNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error UserNotFound(string userId) =>
        Error.NotFound("Identity.UserNotFound", $"User with ID '{userId}' not found");

    //-------------------------------------------------------------------------------
    //
    //                       UserNotInRole
    //
    //-------------------------------------------------------------------------------
    public static Error UserNotInRole(string userId, string role) =>
        Error.Failure("Identity.UserNotInRole", $"User '{userId}' is not in role '{role}'");


    //-------------------------------------------------------------------------------
    //
    //                       RemoveFromRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error RemoveFromRoleFailed()
    {
      var error = Error.Failure("Identity.RemoveFromRoleFailed",
          $"Failed to remove user from role.");
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       AddToRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error AddToRoleFailed()
    {
      var error = Error.Failure(
        "Identity.AddToRoleFailed",
        $"Failed to add user to role.");
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       InvalidArgument
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidArgument(string description) =>
        Error.Failure("Identity.InvalidArgument", description);

    //-------------------------------------------------------------------------------
    //
    //                       DuplicateUserName
    //
    //-------------------------------------------------------------------------------
    public static Error DuplicateUserName(string userName) =>
        Error.Failure("Identity.DuplicateUserName", $"User name '{userName}' is already taken");

    //-------------------------------------------------------------------------------
    //
    //                       DuplicateEmail
    //
    //-------------------------------------------------------------------------------
    public static Error DuplicateEmail(string email) =>
        Error.Failure("Identity.DuplicateEmail", $"Email '{email}' is already taken");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidUserName
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidUserName(string userName) =>
        Error.Failure("Identity.InvalidUserName", $"User name '{userName}' is invalid");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidEmail
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidEmail(string email) =>
        Error.Failure("Identity.InvalidEmail", $"Email '{email}' is not valid");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidEmail
    //
    //-------------------------------------------------------------------------------
    public static Error CreateUserFailed(string userName, string email, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure("Identity.CreateUserFailed",
          $"Failed to create user '{userName}' with email '{email}'.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       AddToRolesFailed
    //
    //-------------------------------------------------------------------------------
    public static Error AddToRolesFailed(string userId, IEnumerable<string> roles, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure("Identity.AddToRolesFailed",
          $"Failed to add roles '{string.Join(", ", roles)}' to user '{userId}'.");
      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       DuplicateRoleName
    //
    //-------------------------------------------------------------------------------
    public static Error DuplicateRoleName(string roleName) =>
        Error.Failure("Identity.DuplicateRoleName", $"Role name '{roleName}' already exists.");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidRoleName
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidRoleName(string roleName) =>
        Error.Failure("Identity.InvalidRoleName", $"Role name '{roleName}' is invalid.");

    //-------------------------------------------------------------------------------
    //
    //                       InvalidOperation
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidOperation(string description) =>
        Error.Failure(
          "Identity.InvalidOperation", 
          description);

    //-------------------------------------------------------------------------------
    //
    //                       GenericIdentityError
    //
    //-------------------------------------------------------------------------------
    public static Error GenericIdentityError(
      string userId,
      IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure(
        code: "Identity.Error",
        description: $"An operation error has occurred for user: '{userId}'.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       UserLockedOut
    //
    //-------------------------------------------------------------------------------
    public static Error UserLockedOut(string email)
    {
      var error = Error.Unauthorized(
        code: "Identity.UserLockedOut",
        description: $"The user: '{email}' has been locked out.");
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       IsNotAllowed
    //
    //-------------------------------------------------------------------------------
    public static Error IsNotAllowed(string email)
    {
      var error = Error.Unauthorized(
        code: "Identity.UserNotAllowed",
        description: $"The user: '{email}' is not allowed to log in.");
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       RequiresTwoFactor
    //
    //-------------------------------------------------------------------------------
    public static Error RequiresTwoFactor(string email)
    {
      var error = Error.Unauthorized(
        code: "Identity.RequiresTwoFactor",
        description: $"The user: '{email}' requires two-factor authentication.");
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       InvalidCredentials
    //
    //-------------------------------------------------------------------------------
    public static Error InvalidCredentials(string email)
    {
      var error = Error.Unauthorized(
        code: "Identity.InvalidCredentials",
        description: $"The user: '{email}' has invalid credentials.");
      return error;
    }

    //-------------------------------------------------------------------------------
    //
    //                       Unauthorized
    //
    //-------------------------------------------------------------------------------
    public static ErrorOr.Error Unauthorized() =>
        Error.Unauthorized(
          code: "Identity.Unauthorized",
          description: "User is not authorized.");

    //-------------------------------------------------------------------------------
    //
    //                       PasswordTooShort
    //
    //-------------------------------------------------------------------------------
    public static ErrorOr.Error PasswordTooShort() =>
        Error.Unauthorized(
          code: "Identity.PasswordTooShort",
          description: $"Password must be at least {_configParams.IdentitySettings.Password.RequiredLength} characters.");

    //-------------------------------------------------------------------------------
    //
    //                       PasswordRequiresNonAlphanumeric
    //
    //-------------------------------------------------------------------------------
    public static ErrorOr.Error PasswordRequiresNonAlphanumeric() =>
        Error.Unauthorized(
          code: "Identity.PasswordRequiresNonAlphanumeric",
          description: "Password must contain at least one non-alphanumeric character (e.g., !, @, #).");

    //-------------------------------------------------------------------------------
    //
    //                       PasswordRequiresDigit
    //
    //-------------------------------------------------------------------------------
    public static ErrorOr.Error PasswordRequiresDigit() =>
        Error.Unauthorized(
          code: "Identity.PasswordRequiresDigit",
          description: "Password must contain at least one digit (0-9).");

    //-------------------------------------------------------------------------------
    //
    //                       PasswordRequiresLower
    //
    //-------------------------------------------------------------------------------
    public static ErrorOr.Error PasswordRequiresLower() =>
        Error.Unauthorized(
          code: "Identity.PasswordRequiresLower",
          description: "Password must contain at least one lowercase letter (a-z).");

    //-------------------------------------------------------------------------------
    //
    //                       PasswordRequiresUpper
    //
    //-------------------------------------------------------------------------------
    public static ErrorOr.Error PasswordRequiresUpper() =>
        Error.Unauthorized(
          code: "Identity.PasswordRequiresUpper",
          description: "Password must contain at least one uppercase letter (A-Z).");

    //-------------------------------------------------------------------------------
    //
    //                       PasswordRequiresUniqueChars
    //
    //-------------------------------------------------------------------------------
    public static ErrorOr.Error PasswordRequiresUniqueChars() =>
        Error.Unauthorized(
          code: "Identity.PasswordRequiresUniqueChars",
          description: "Password must contain at least one non-alphanumeric character (e.g., !, @, #).");

    //-------------------------------------------------------------------------------
    //
    //                       AddToRolesError
    //
    //-------------------------------------------------------------------------------
    public static ErrorOr.Error AddToRolesError() =>
        Error.Unauthorized(
          code: "Identity.AddToRolesError",
          description: "Failed to add roles: Invalid user or roles.");
  }
}

