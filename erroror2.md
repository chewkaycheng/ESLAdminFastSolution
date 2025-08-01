Yes. **ErrorOr** lets you avoid stringly-typed `error.Code` switches by defining distinct error types—i.e., your own `IError` implementations—so you can pattern-match on the type instead of inspecting the code.

Here’s a minimal pattern using typed errors:

```csharp
// 1. Define domain-specific error types
public abstract record UserError : IError
{
    public abstract string Code { get; }
    public abstract string Description { get; }
}

public sealed record UserNotFoundError(string Email) : UserError
{
    public override string Code => "User.NotFound";
    public override string Description => $"The user with email {Email} is not found.";
}

public sealed record RemoveRolesFailedError(string Email, IEnumerable<IdentityError> IdentityErrors) : UserError
{
    public override string Code => "User.RemoveRolesFailed";
    public override string Description =>
        $"Failed to remove roles for {Email}: {string.Join("; ", IdentityErrors.Select(e => e.Description))}";

    public IdentityError[] RawErrors => IdentityErrors.ToArray();
}

public sealed record DeleteUserFailedError(string Email, IEnumerable<IdentityError> IdentityErrors) : UserError
{
    public override string Code => "User.DeleteFailed";
    public override string Description =>
        $"Failed to delete user {Email}: {string.Join("; ", IdentityErrors.Select(e => e.Description))}";

    public IdentityError[] RawErrors => IdentityErrors.ToArray();
}

public sealed record ExceptionError(string Message) : UserError
{
    public override string Code => "User.Exception";
    public override string Description => Message;
}
```

```csharp
// 2. Use those in your method
public async Task<ErrorOr<string>> DeleteUserByEmailAsync(string email)
{
    try
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new UserNotFoundError(email);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Any())
        {
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!removeRolesResult.Succeeded)
            {
                InfoLogIdentityErrors("RemoveFromRolesAsync", email, removeRolesResult.Errors);
                return new RemoveRolesFailedError(email, removeRolesResult.Errors);
            }
        }

        var deleteUserResult = await _userManager.DeleteAsync(user);
        if (!deleteUserResult.Succeeded)
        {
            InfoLogIdentityErrors("DeleteAsync", email, deleteUserResult.Errors);
            return new DeleteUserFailedError(email, deleteUserResult.Errors);
        }

        return user.Id; // success
    }
    catch (Exception ex)
    {
        _logger.LogException(ex);
        return new ExceptionError(ex.Message);
    }
}
```

```csharp
// 3. Caller can now switch on the error *type*
var result = await repo.DeleteUserByEmailAsync(email);
if (result.IsError)
{
    foreach (var error in result.Errors)
    {
        switch (error)
        {
            case UserNotFoundError notFound:
                // handle not found
                break;
            case RemoveRolesFailedError removeRoles:
                var roleErrors = removeRoles.RawErrors;
                // handle role removal failure
                break;
            case DeleteUserFailedError deleteFailed:
                var delErrors = deleteFailed.RawErrors;
                // handle deletion failure
                break;
            case ExceptionError exError:
                // handle unexpected exception
                break;
            default:
                // fallback for other IError implementations
                break;
        }
    }
}
else
{
    string userId = result.Value;
    // success
}
```

**Why this works well:**

* You get **compile-time type distinction** instead of string code comparisons.
* Each error can carry strongly-typed payloads (e.g., the original `IdentityError[]`).
* The `IError` interface is what `ErrorOr<T>` expects, so these records integrate seamlessly.

If you prefer not to create many concrete types, a lighter-weight alternative is to keep using `Error` but attach an enum or discriminant in metadata (e.g., `"Step" : "RemoveRoles"`), then switch on that metadata. But the typed-record approach above is cleaner and avoids brittle string logic.
