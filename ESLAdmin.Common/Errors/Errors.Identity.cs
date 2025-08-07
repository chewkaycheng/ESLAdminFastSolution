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
    //                       RefreshTokenNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error RefreshTokenNotFound(string token) =>
      Error.Validation(
        code: "User.RefreshTokenNotFound",
        description: $"The refresh token: '{token}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       AddToRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error AddToRoleFailed(string email, string roleName, IEnumerable<IdentityError>? errors = null)
    {
      var error = Error.Failure(
        code: "User.AddToRoleFailed",
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
        code: "User.InvalidRoles",
        description: $"The following roles do not exist: {string.Join(", ", invalidRoles)}");


    //-------------------------------------------------------------------------------
    //
    //                       UserEmailCannotBeEmpty
    //
    //-------------------------------------------------------------------------------
    public static Error UserEmailCannotBeEmpty() =>
      Error.Validation(
        code: "User.UserEmailEmpty",
        description: $"The email for the user cannot be empty.");

    //-------------------------------------------------------------------------------
    //
    //                       UserEmailNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error UserEmailNotFound(string email) =>
      Error.Validation(
        code: "User.NotFound",
        description: $"The user with email: '{email}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       UserIdNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error UserIdNotFound(string userId) =>
      Error.Validation(
        code: "User.NotFound",
        description: $"The user with Id: '{userId}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       UserNameNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error UserNameNotFound(string username) =>
      Error.Validation(
        code: "User.NotFound",
        description: $"The user with Username: '{username}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       UserLoginFailed
    //
    //-------------------------------------------------------------------------------
    public static Error UserLoginFailed() =>
      Error.Validation(
        code: "User.LoginFailed",
        description: $"Username or password is invalid.");

    //-------------------------------------------------------------------------------
    //
    //                       CreateUserFailed
    //
    //-------------------------------------------------------------------------------
    public static Error CreateUserFailed(string email, IEnumerable<IdentityError> errors)
    {
      var error = Error.Failure(
        code: "User.CreateFailed",
        description: $"Failed to remove user: '{email}'.");

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
        code: "User.AddToRolesFailed",
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
        code: "User.RemoveRolesFailed",
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
        code: "User.DeleteFailed",
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
        code: "User.AssignRoleFailed",
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
        code: "User.UserAlreadyInRole",
        description: $"The role: '{roleName}' is already assigned to the user: '{email}'.");

    //-------------------------------------------------------------------------------
    //
    //                       RoleNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error RoleNotFound(string roleName) =>
      Error.Validation(
        code: "Role.NotFound",
        description: $"The role: '{roleName}' is not found.");

    //-------------------------------------------------------------------------------
    //
    //                       RoleExists
    //
    //-------------------------------------------------------------------------------
    public static Error RoleExists(string roleName) =>
      Error.Conflict(
        code: "Role.Exists",
        description: $"The role: '{roleName}' already exists.");

    //-------------------------------------------------------------------------------
    //
    //                       CreateRoleFailed
    //
    //-------------------------------------------------------------------------------
    public static Error CreateRoleFailed(string roleName, IEnumerable<IdentityError> errors)
    {

      var error = Error.Failure(
        code: "Role.CreateFailed",
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
        code: "Role.UpdateFailed",
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
        code: "Role.DeleteFailed",
        description: $"Failed to delete role: '{roleName}'.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

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
}
