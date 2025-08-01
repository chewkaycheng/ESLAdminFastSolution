You’re almost there; with **ErrorOr** the clean way is to wrap each failure case in a distinct `Error` (with a code or type) so the caller can pattern-match or inspect which step failed, while still preserving the underlying `IdentityError[]` details.

Here’s a sketch of how to refactor your method to return `ErrorOr<string>` (the `string` being the user.Id on success), with distinguishable errors for the “remove roles” vs “delete user” cases:

```csharp
public static class Errors
{
    public static Error UserNotFound(string email) =>
        Error.Validation(
            code: "User.NotFound",
            description: $"The user with email {email} is not found.");

    public static Error RemoveRolesFailed(string email, IEnumerable<IdentityError> identityErrors) =>
        Error.Failure(
            code: "User.RemoveRolesFailed",
            description: $"Failed to remove roles for {email}: {string.Join("; ", identityErrors.Select(e => e.Description))}")
            .WithMetadata("IdentityErrors", identityErrors.ToArray());

    public static Error DeleteUserFailed(string email, IEnumerable<IdentityError> identityErrors) =>
        Error.Failure(
            code: "User.DeleteFailed",
            description: $"Failed to delete user {email}: {string.Join("; ", identityErrors.Select(e => e.Description))}")
            .WithMetadata("IdentityErrors", identityErrors.ToArray());

    public static Error Exception(string message) =>
        Error.Failure(
            code: "User.Exception",
            description: message);
}

public async Task<ErrorOr<string>> DeleteUserByEmailAsync(string email)
{
    try
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Errors.UserNotFound(email);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Any())
        {
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!removeRolesResult.Succeeded)
            {
                InfoLogIdentityErrors("RemoveFromRolesAsync", email, removeRolesResult.Errors);
                return Errors.RemoveRolesFailed(email, removeRolesResult.Errors);
            }
        }

        var deleteUserResult = await _userManager.DeleteAsync(user);
        if (!deleteUserResult.Succeeded)
        {
            InfoLogIdentityErrors("DeleteAsync", email, deleteUserResult.Errors);
            return Errors.DeleteUserFailed(email, deleteUserResult.Errors);
        }

        return user.Id; // success
    }
    catch (Exception ex)
    {
        _logger.LogException(ex);
        return Errors.Exception(ex.Message);
    }
}
```

**Notes:**

* `Error.Failure` / `Error.Validation` (depending on what semantics you want) gives each failure a distinct code like `"User.RemoveRolesFailed"` vs `"User.DeleteFailed"`.
* I attached the raw `IdentityError[]` into metadata (`WithMetadata`) so callers that need the granular error objects can retrieve them.
* You can also create typed wrappers instead of raw metadata if you prefer (e.g., a custom `Error` subclass or helper to extract `IdentityError[]`).

Caller example:

```csharp
var result = await repository.DeleteUserByEmailAsync(email);
if (result.IsError)
{
    foreach (var error in result.Errors)
    {
        switch (error.Code)
        {
            case "User.RemoveRolesFailed":
                var identityErrors = error.Metadata["IdentityErrors"] as IdentityError[];
                // handle role removal failure
                break;
            case "User.DeleteFailed":
                var deleteErrors = error.Metadata["IdentityErrors"] as IdentityError[];
                // handle deletion failure
                break;
            case "User.NotFound":
                // handle not found
                break;
            default:
                // other/fallback
                break;
        }
    }
}
else
{
    string userId = result.Value;
    // success path
}
```

If you still want to keep `IdentityResultEx` as your return type, you can adapt the same idea: give it an enum or code property indicating the step (`RemoveRoles`, `DeleteUser`, etc.) and include the underlying `IdentityError[]` so the caller can distinguish. But with **ErrorOr** it’s idiomatic to use distinct `Error` codes and metadata.

Do you want a version that preserves `IdentityResultEx` and wraps `ErrorOr<IdentityResultEx>` instead, or are you okay switching to `ErrorOr<string>` (user ID) as above?

