To address your request, I'll create the necessary classes for handling the user list retrieval using FastEndpoints, Dapper for querying the Firebird database, and a stored procedure to fetch user roles with their descriptions. I'll also use LINQ to link users with their roles and return a response containing user information and their associated roles. Since Dapper is simpler for executing raw SQL queries and stored procedures, I'll use it instead of Entity Framework for the database operations in this context.

Below are the implementations for the requested classes, including a stored procedure for the usersroles table joined with the roles table. I'll assume the database schema includes tables `Users`, `UserRoles`, and `Roles` (based on ASP.NET Identity conventions for Firebird), and I'll create a stored procedure to fetch user roles with role descriptions.

---

```x-csharp

using ESLAdmin.Features.Users.Models;
using FastEndpoints;

namespace ESLAdmin.Features.Users;

public class UserList : Endpoint<UserListRequest, UserListResponse>
{
    public override void Configure()
    {
        Get("/api/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UserListRequest req, CancellationToken ct)
    {
        var command = new UserListCommand();
        var response = await SendAsync<UserListResponse>(command, cancellation: ct);
        await SendAsync(response, cancellation: ct);
    }
}

```

```x-csharp

namespace ESLAdmin.Features.Users;

public class UserListCommand
{
    // No properties needed as this command retrieves all users
}

```

```x-csharp

using Dapper;
using ESLAdmin.Common.Exceptions;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Infrastructure.Data;
using ESLAdmin.Logging.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace ESLAdmin.Features.Users;

public class UserListCommandHandler : ICommandHandler<UserListCommand, UserListResponse>
{
    private readonly IDbConnection _dbConnection;
    private readonly IMessageLogger _messageLogger;
    private readonly ILogger _logger;

    public UserListCommandHandler(
        IDbConnection dbConnection,
        IMessageLogger messageLogger,
        ILogger<UserListCommandHandler> logger)
    {
        _dbConnection = dbConnection;
        _messageLogger = messageLogger;
        _logger = logger;
    }

    public async Task<UserListResponse> ExecuteAsync(UserListCommand command, CancellationToken ct)
    {
        try
        {
            // Query to get all users
            const string userSql = "SELECT Id, UserName, Email FROM Users";
            var users = await _dbConnection.QueryAsync<UserDto>(userSql);

            // Query to get user roles with role descriptions using stored procedure
            const string rolesSql = "EXECUTE PROCEDURE GetUserRolesWithDescriptions";
            var userRoles = await _dbConnection.QueryAsync<UserRoleDto>(rolesSql);

            // Group user roles by UserId and map to response
            var userRoleLookup = userRoles
                .GroupBy(ur => ur.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(ur => new RoleDto { RoleId = ur.RoleId, RoleName = ur.RoleName }).ToList()
                );

            // Map users to response, linking roles
            var userList = users.Select(u => new UserWithRolesDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Roles = userRoleLookup.TryGetValue(u.Id, out var roles) ? roles : new List<RoleDto>()
            }).ToList();

            return new UserListResponse { Users = userList };
        }
        catch (Exception ex)
        {
            _messageLogger.LogDatabaseException(nameof(UserListCommandHandler), ex);
            throw new DatabaseException(nameof(UserListCommandHandler), ex);
        }
    }
}

```

```x-csharp

using ESLAdmin.Features.Users.Models;
using FastEndpoints;

namespace ESLAdmin.Features.Users;

public class UserListMapper : ResponseMapper<UserListResponse, UserListCommand>
{
    public override UserListResponse FromEntity(UserListCommand e) => new();
}

```

```x-csharp

using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Features.Users.Models;

public class UserListRequest
{
    // No request parameters needed for listing all users
}

public class UserListResponse
{
    public List<UserWithRolesDto> Users { get; set; } = new();
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UserRoleDto
{
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}

public class RoleDto
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}

public class UserWithRolesDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<RoleDto> Roles { get; set; } = new();
}

```

```sql

CREATE PROCEDURE GetUserRolesWithDescriptions
RETURNS (
    UserId VARCHAR(36),
    RoleId VARCHAR(36),
    RoleName VARCHAR(256)
)
AS
BEGIN
    FOR SELECT ur.UserId, ur.RoleId, r.Name
    FROM UserRoles ur
    INNER JOIN Roles r ON ur.RoleId = r.Id
    INTO :UserId, :RoleId, :RoleName
    DO
        SUSPEND;
END

```

---

### Explanation of the Implementation

1. **Stored Procedure (`GetUserRolesWithDescriptions.sql`)**:
   - Created a Firebird stored procedure to join the `UserRoles` and `Roles` tables.
   - Returns `UserId`, `RoleId`, and `RoleName` (role description) for each user-role mapping.
   - Uses Firebird SQL syntax with `SUSPEND` to return rows iteratively.

2. **UserList Endpoint (`UserList.cs`)**:
   - Defines a FastEndpoints GET endpoint at `/api/users`.
   - Allows anonymous access (modify as needed for authorization).
   - Sends a `UserListCommand` and returns the `UserListResponse`.

3. **Command (`UserList.Command.cs`)**:
   - A simple command class with no properties, as it retrieves all users without parameters.

4. **Command Handler (`UserList.CommandHandler.cs`)**:
   - Uses Dapper to execute two queries:
     - A direct SQL query to fetch all users (`Users` table: `Id`, `UserName`, `Email`).
     - The stored procedure `GetUserRolesWithDescriptions` to fetch user roles with role names.
   - Uses LINQ to group user roles by `UserId` and create a lookup dictionary.
   - Maps users to `UserWithRolesDto`, linking each user to their roles using the lookup.
   - Handles exceptions with logging and wraps them in a `DatabaseException`.

5. **Mapper (`UserList.Mapper.cs`)**:
   - A minimal FastEndpoints mapper to satisfy the framework's requirements.
   - Since the response is built in the handler, it returns an empty response from the command.

6. **Models (`UserList.Models.cs`)**:
   - `UserListRequest`: Empty, as no input parameters are needed.
   - `UserListResponse`: Contains a list of `UserWithRolesDto`.
   - `UserDto`: Maps to the `Users` table columns.
   - `UserRoleDto`: Maps to the stored procedure output.
   - `RoleDto`: Represents a role with `RoleId` and `RoleName`.
   - `UserWithRolesDto`: Combines user details with a list of roles.

### Notes
- **Database Connection**: The handler assumes an `IDbConnection` (configured for Firebird) is injected. Ensure your dependency injection setup provides this (e.g., via `FirebirdSql.Data.FirebirdClient`).
- **Stored Procedure Execution**: The Firebird stored procedure is executed using Dapper's `QueryAsync`. Ensure the procedure is created in the database before running the application.
- **Error Handling**: Exceptions are logged using `IMessageLogger` and wrapped in `DatabaseException`, consistent with your `AuthenticationRepository`.
- **LINQ Usage**: The LINQ operations group and map user roles efficiently, avoiding N+1 queries by fetching all data upfront.
- **Dapper Choice**: Dapper is used for its simplicity and performance with raw SQL and stored procedures, compared to Entity Framework, which would require additional configuration for stored procedures.

### Prerequisites
- Ensure the Firebird database has the `Users`, `UserRoles`, and `Roles` tables (standard ASP.NET Identity schema).
- Run the `GetUserRolesWithDescriptions.sql` script to create the stored procedure.
- Configure Dapper with Firebird in your dependency injection (e.g., `services.AddScoped<IDbConnection>(sp => new FbConnection(connectionString))`).
- Adjust the SQL queries if your table/column names differ from the ASP.NET Identity defaults.

If you need further modifications or additional details (e.g., database schema, dependency injection setup), please let me know!