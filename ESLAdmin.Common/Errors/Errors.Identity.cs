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
    //                       UserNotFound
    //
    //-------------------------------------------------------------------------------
    public static Error UserNotFound(string email) =>
      Error.Validation(
        code: "User.NotFound",
        description: $"The user with email {email} is not found.");

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
        description: $"Failed to delete user {email}.");

      AddMetadata(error.Metadata, errors);
      return error;
    }

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
