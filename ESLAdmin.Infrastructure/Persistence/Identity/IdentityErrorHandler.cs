using ErrorOr;
using ESLAdmin.Common.Configuration;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Persistence.Identity;

public enum IdentityOperation
{
  AddToRoles,
  AddToRole,
  RemoveFromRole,
  CreateRole,
  DeleteRole,
  UpdateRole,
  DeleteUser,
  CreateUser,
  LoginUser,
  RegisterUser
  // Add future operations here
}

//------------------------------------------------------------------------------
//
//                      static class IdentityErrorHandler
//
//------------------------------------------------------------------------------
public static class IdentityErrorHandler
{
  //------------------------------------------------------------------------------
  //
  //                      HandleIdentityErrors
  //
  //------------------------------------------------------------------------------

  public static Error HandleIdentityErrors(
    IdentityResult result,
    ILogger logger,
    IdentityOperation operation,
    string userId = "", 
    string userName = "",
    string roleName = "",
    string oldRoleName = "",
    string newRoleName = "",
    string roles = "",
    string operationName = "", 
    string email = "")
  {
    logger.LogIdentityErrors(operationName, email, result.Errors.ToFormattedString());
    var firstError = result.Errors.FirstOrDefault();

    return firstError?.Code switch
    {
      "DuplicateUserName" => AppErrors.IdentityErrors.DuplicateUserName(userName),
      "DuplicateEmail" => AppErrors.IdentityErrors.DuplicateEmail(email),
      "InvalidUserName" => AppErrors.IdentityErrors.InvalidUserName(userName),
      "InvalidEmail" => AppErrors.IdentityErrors.InvalidEmail(email ?? "null"),
      "PasswordTooShort" => AppErrors.IdentityErrors.PasswordTooShort(),
      "PasswordRequiresNonAlphanumeric" => 
        AppErrors.IdentityErrors.PasswordRequiresNonAlphanumeric(),
      "PasswordRequiresDigit" =>
        AppErrors.IdentityErrors.PasswordRequiresDigit(),
      "PasswordRequiresLower" =>
        AppErrors.IdentityErrors.PasswordRequiresLower(),
      "PasswordRequiresUpper" =>
        AppErrors.IdentityErrors.PasswordRequiresUpper(),


      "UserNotFound" => AppErrors.IdentityErrors.UserNotFound(userId),
      "RoleNotFound" => AppErrors.IdentityErrors.RoleNotFound(roleName),
      "DuplicateRoleName" => AppErrors.IdentityErrors.DuplicateRoleName(roleName),
      "InvalidRoleName" => AppErrors.IdentityErrors.InvalidRoleName(roleName),
      "UserNotInRole" => AppErrors.IdentityErrors.UserNotInRole(userId, roleName),
      "UserAlreadyInRole" => AppErrors.IdentityErrors.UserAlreadyInRole(userId, roleName),
      "ConcurrencyFailure" => AppErrors.IdentityErrors.ConcurrencyFailure(),
      "UserLockedOut" => AppErrors.IdentityErrors.UserLockedOut(email),
      "IsNotAllowed" => AppErrors.IdentityErrors.IsNotAllowed(email),
      "RequiresTwoFactor" => AppErrors.IdentityErrors.RequiresTwoFactor(email),

      _ => operation switch
      {
        IdentityOperation.AddToRole => AppErrors.IdentityErrors.AddToRoleFailed(userId, roleName, result.Errors),
        IdentityOperation.AddToRoles => AppErrors.IdentityErrors.AddToRolesFailed(userId, result.Errors),
        IdentityOperation.RemoveFromRole => AppErrors.IdentityErrors.RemoveFromRoleFailed(userId, roleName, result.Errors),
        IdentityOperation.CreateRole => AppErrors.IdentityErrors.CreateRoleFailed(roleName, result.Errors),
        IdentityOperation.DeleteRole => AppErrors.IdentityErrors.DeleteRoleFailed(roleName, result.Errors),
        IdentityOperation.DeleteUser => AppErrors.IdentityErrors.DeleteUserFailed(userId, result.Errors),
        IdentityOperation.CreateUser => AppErrors.IdentityErrors.CreateUserFailed(userId, email, result.Errors),
        IdentityOperation.UpdateRole => AppErrors.IdentityErrors.UpdateRoleFailed(oldRoleName, newRoleName, result.Errors),
        IdentityOperation.LoginUser => AppErrors.IdentityErrors.InvalidCredentials(email),
      }
    };
  }
}
