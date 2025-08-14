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
    public static Error DeleteUserFailed(string email, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure(
        code: "Identity.DeleteUserFailed",
        description: $"Failed to delete user: '{email}'.");

      AddMetadata(error.Metadata, errors);
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
    //                       ConcurrencyFailure
    //
    //-------------------------------------------------------------------------------
    public static Error ConcurrencyFailure(string userId) =>
        Error.Failure("Identity.ConcurrencyFailure", $"Concurrency failure for user '{userId}'");

    //-------------------------------------------------------------------------------
    //
    //                       RemoveFromRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error RemoveFromRoleFailed(string userId, string role, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure("Identity.RemoveFromRoleFailed",
          $"Failed to remove role '{role}' from user '{userId}.");
      AddMetadata(error.Metadata, errors);
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
  }
}

