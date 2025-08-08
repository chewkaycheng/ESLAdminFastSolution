To address your request, I'll design an `IConfigurationParams` interface and a `ConfigurationParams` class as a singleton that centralizes configuration parameter management. The class will read configuration settings into a dictionary during construction, validate them, and be instantiated in `Program.cs` to ensure the application fails fast if any parameters are missing. The solution will respect your existing architecture (ASP.NET Core 9.0, FastEndpoints, Dapper, EF Core, Firebird, and the `ESLAdmin` namespace structure) and ensure compatibility with dependency injection (DI) and your global exception handler. I'll also specify where to place the interface and class, provide their implementations, and show how to integrate them into `Program.cs` and other classes (e.g., `RefreshTokenCommandHandler`).

### Goals
- Create an `IConfigurationParams` interface and `ConfigurationParams` class to manage configuration settings in a dictionary.
- Implement `ConfigurationParams` as a singleton that reads and validates configuration parameters in its constructor.
- Validate parameters in `Program.cs` and stop the application if any are missing, ensuring the global exception handler doesn’t interfere.
- Make `IConfigurationParams` injectable via DI for use in command handlers and endpoints.
- Place the interface and class in an appropriate location within the `ESLAdmin` project structure.
- Ensure nullability warnings are handled (assuming `<Nullable>enable</Nullable>`).
- Maintain consistency with your existing code (e.g., JWT configuration keys, logging, FastEndpoints).

### Solution

#### 1. Define the Interface and Class
The `IConfigurationParams` interface will expose a read-only dictionary of configuration settings and a validation method. The `ConfigurationParams` class will implement this interface, read configuration values, and validate them during construction.

**File**: `ESLAdmin.Infrastructure.Configuration/IConfigurationParams.cs`

```csharp
namespace ESLAdmin.Infrastructure.Configuration;

public interface IConfigurationParams
{
    IReadOnlyDictionary<string, string> Settings { get; }
    void ValidateConfiguration(ILogger logger, params string[] requiredKeys);
}
```

**File**: `ESLAdmin.Infrastructure.Configuration/ConfigurationParams.cs`

```csharp
namespace ESLAdmin.Infrastructure.Configuration;

public class ConfigurationParams : IConfigurationParams
{
    private readonly Dictionary<string, string> _settings;

    public IReadOnlyDictionary<string, string> Settings => _settings;

    public ConfigurationParams(IConfiguration config)
    {
        _settings = new Dictionary<string, string>
        {
            { "Jwt:Key", config["Jwt:Key"] ?? "" },
            { "Jwt:Issuer", config["Jwt:Issuer"] ?? "" },
            { "Jwt:Audience", config["Jwt:Audience"] ?? "" }
            // Add other configuration keys as needed
        };
    }

    public void ValidateConfiguration(ILogger logger, params string[] requiredKeys)
    {
        var missingKeys = requiredKeys
            .Where(key => !_settings.ContainsKey(key) || string.IsNullOrEmpty(_settings[key]))
            .ToList();

        if (missingKeys.Any())
        {
            foreach (var key in missingKeys)
            {
                logger.LogError("{ConfigKey} is not found or empty in the configuration", key);
            }
            throw new InvalidOperationException(
                $"Missing or empty configuration keys: {string.Join(", ", missingKeys)}");
        }
    }
}
```

**Key Features**:
- **Interface**:
  - `Settings`: Exposes a read-only dictionary of configuration values.
  - `ValidateConfiguration`: Validates a subset of required keys, logging errors and throwing an exception if any are missing or empty.
- **Class**:
  - Constructor: Reads configuration values from `IConfiguration`, using `?? ""` to handle nulls and avoid nullability warnings.
  - Singleton: Will be registered as a singleton in DI to ensure a single instance.
  - Validation: Throws an `InvalidOperationException` if any required keys are missing, ensuring the application stops in `Program.cs`.
- **Nullability**: Uses non-nullable `string` in the dictionary to align with your preference for non-nullable values (as seen in previous dictionary-based validation).

**Why This Location**:
- **Namespace**: `ESLAdmin.Infrastructure.Configuration` is a logical place for configuration-related utilities, aligning with your `ESLAdmin.Infrastructure` layer (used for `RepositoryManager`, logging, etc.).
- **Separation of Concerns**: Keeps configuration management separate from business logic (`ESLAdmin.Features`) and data access (`ESLAdmin.Infrastructure.Repositories`).
- **Reusability**: Accessible to all parts of the application (endpoints, command handlers, services) via DI.

#### 2. Register and Validate in Program.cs
Instantiate `ConfigurationParams` as a singleton in `Program.cs` and validate the configuration during application startup. To ensure the application stops if validation fails, throw an exception that bypasses the global exception handler.

**File**: `Program.cs`

```csharp
using ESLAdmin.Infrastructure.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IConfigurationParams, ConfigurationParams>();

// Add other services (e.g., FastEndpoints, EF Core, Dapper, logging)
builder.Services.AddFastEndpoints();
builder.Services.AddLogging();

// Validate configuration at startup
var configParams = builder.Services.BuildServiceProvider()
    .GetRequiredService<IConfigurationParams>();
var logger = builder.Services.BuildServiceProvider()
    .GetRequiredService<ILogger<Program>>();

configParams.ValidateConfiguration(
    logger,
    "Jwt:Key",
    "Jwt:Issuer",
    "Jwt:Audience");

var app = builder.Build();

// Configure global exception handler (example)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "Unhandled exception in {RequestPath}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred."
            });
        }
    });
});

// Configure other middleware (e.g., FastEndpoints, routing)
app.UseFastEndpoints();

app.Run();
```

**Key Features**:
- **Singleton Registration**: Registers `IConfigurationParams` as a singleton with `ConfigurationParams` implementation.
- **Early Validation**: Calls `ValidateConfiguration` before `app.Build()` to ensure the application doesn’t start if configuration is invalid.
- **Exception Handling**: The `InvalidOperationException` thrown by `ValidateConfiguration` will terminate the application before the global exception handler is active (since it’s before `app.Run()`).
- **Logger**: Uses `ILogger<Program>` for startup logging, consistent with your existing logging setup.

**Why This Works**:
- The exception in `ValidateConfiguration` stops the application immediately during startup, bypassing the global exception handler (which only applies to HTTP requests after `app.Build()`).
- The singleton ensures a single instance of `ConfigurationParams` is available for DI throughout the application.

#### 3. Use in RefreshTokenCommandHandler
Inject `IConfigurationParams` into `RefreshTokenCommandHandler` and use the pre-validated `Settings` dictionary, eliminating the need for runtime validation.

**File**: `ESLAdmin.Features.Endpoints.Users/RefreshTokenCommandHandler.cs`

```csharp
using ESLAdmin.Infrastructure.Configuration;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

public class RefreshTokenCommand
{
    // Define properties as needed
}

public class RefreshTokenResponse
{
    // Define properties as needed
}

public class RefreshTokenCommandHandler : ICommandHandler<
    RefreshTokenCommand,
    Results<Ok<RefreshTokenResponse>, ProblemDetails, InternalServerError>>
{
    private readonly IConfigurationParams _configParams;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly IRepositoryManager _repositoryManager;

    public RefreshTokenCommandHandler(
        IConfigurationParams configParams,
        ILogger<RefreshTokenCommandHandler> logger,
        IRepositoryManager repositoryManager)
    {
        _configParams = configParams;
        _logger = logger;
        _repositoryManager = repositoryManager;
    }

    public async Task<Results<Ok<RefreshTokenResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var jwtKey = _configParams.Settings["Jwt:Key"];
        var issuer = _configParams.Settings["Jwt:Issuer"];
        var audience = _configParams.Settings["Jwt:Audience"];

        // Proceed with refresh token logic
        // Example: var token = await _repositoryManager.AuthenticationRepository.RefreshTokenAsync(command);
        return TypedResults.Ok(new RefreshTokenResponse());
    }
}
```

**Key Features**:
- **DI**: Injects `IConfigurationParams` instead of `IConfiguration`, accessing validated settings via `Settings`.
- **No Validation Needed**: Since `Program.cs` validates the configuration at startup, the handler can assume `Jwt:Key`, `Jwt:Issuer`, and `Jwt:Audience` exist and are non-empty.
- **Type Safety**: `Settings` is a `IReadOnlyDictionary<string, string>`, ensuring non-nullable values (since `ConfigurationParams` uses `?? ""`).

#### 4. Directory Structure
Here’s where to place the files within your project:

```
ESLAdmin/
├── ESLAdmin.Infrastructure/
│   ├── Configuration/
│   │   ├── IConfigurationParams.cs
│   │   ├── ConfigurationParams.cs
│   ├── RepositoryManagers/
│   │   ├── RepositoryManager.cs
│   ├── Logging/
│   │   ├── ...
├── ESLAdmin.Features/
│   ├── Endpoints/
│   │   ├── Users/
│   │   │   ├── RefreshTokenCommandHandler.cs
│   │   │   ├── GetUserEndpoint.cs
│   │   │   ├── ...
├── Program.cs
```

- **IConfigurationParams.cs**: Defines the interface in `ESLAdmin.Infrastructure.Configuration`.
- **ConfigurationParams.cs**: Implements the singleton in `ESLAdmin.Infrastructure.Configuration`.
- **Why This Location**:
  - The `Infrastructure` layer is ideal for cross-cutting concerns like configuration, logging, and repository management (as seen with `RepositoryManager` and `IMessageLogger`).
  - A dedicated `Configuration` subfolder keeps configuration-related code organized.
  - Aligns with your existing use of `ESLAdmin.Infrastructure` for utilities.

#### 5. Ensuring Application Stops
To ensure the application stops if configuration validation fails:
- The `InvalidOperationException` thrown by `ValidateConfiguration` in `Program.cs` occurs before the web host is built, so the global exception handler (configured in `app.UseExceptionHandler`) won’t catch it.
- This causes the application to exit with a non-zero status code, logging the error to the console or configured logger (e.g., `ILogger<Program>`).
- Example console output:
  ```
  crit: Program[0]
        Missing or empty configuration keys: Jwt:Key, Jwt:Issuer
  Unhandled exception. System.InvalidOperationException: Missing or empty configuration keys: Jwt:Key, Jwt:Issuer
  ```

#### 6. Additional Recommendations
1. **Configuration Binding**:
   - If your configuration is structured (e.g., `"Jwt": { "Key": "...", "Issuer": "...", "Audience": "..." }`), consider binding to a model:
     ```csharp
     public class JwtSettings
     {
         public string Key { get; set; } = "";
         public string Issuer { get; set; } = "";
         public string Audience { get; set; } = "";
     }

     public class ConfigurationParams : IConfigurationParams
     {
         private readonly Dictionary<string, string> _settings;

         public IReadOnlyDictionary<string, string> Settings => _settings;

         public ConfigurationParams(IConfiguration config)
         {
             var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
             _settings = new Dictionary<string, string>
             {
                 { "Jwt:Key", jwtSettings.Key },
                 { "Jwt:Issuer", jwtSettings.Issuer },
                 { "Jwt:Audience", jwtSettings.Audience }
             };
         }

         public void ValidateConfiguration(ILogger logger, params string[] requiredKeys)
         {
             var missingKeys = requiredKeys
                 .Where(key => !_settings.ContainsKey(key) || string.IsNullOrEmpty(_settings[key]))
                 .ToList();

             if (missingKeys.Any())
             {
                 foreach (var key in missingKeys)
                 {
                     logger.LogError("{ConfigKey} is not found or empty in the configuration", key);
                 }
                 throw new InvalidOperationException(
                     $"Missing or empty configuration keys: {string.Join(", ", missingKeys)}");
             }
         }
     }
     ```
   - This is more maintainable for structured configurations and reduces key string literals.

2. **Expand Configuration Keys**:
   - Add other critical configuration keys (e.g., database connection strings) to `_settings` in the constructor:
     ```csharp
     _settings = new Dictionary<string, string>
     {
         { "Jwt:Key", config["Jwt:Key"] ?? "" },
         { "Jwt:Issuer", config["Jwt:Issuer"] ?? "" },
         { "Jwt:Audience", config["Jwt:Audience"] ?? "" },
         { "ConnectionStrings:Firebird", config["ConnectionStrings:Firebird"] ?? "" }
     };
     ```
   - Update `Program.cs` to validate all required keys:
     ```csharp
     configParams.ValidateConfiguration(
         logger,
         "Jwt:Key",
         "Jwt:Issuer",
         "Jwt:Audience",
         "ConnectionStrings:Firebird");
     ```

3. **Structured Logging**:
   - Enhance logging with structured properties:
     ```csharp
     logger.LogError("Missing configuration keys: {@MissingKeys}", missingKeys);
     ```

4. **Testing**:
   - Write unit tests for `ConfigurationParams.ValidateConfiguration` to simulate missing keys and verify the exception is thrown.
   - Test `RefreshTokenCommandHandler` to ensure it uses `Settings` correctly.
   - Example test:
     ```csharp
     [Fact]
     public void ValidateConfiguration_Throws_WhenKeysMissing()
     {
         var config = new ConfigurationBuilder()
             .AddInMemoryCollection(new Dictionary<string, string> { { "Jwt:Issuer", "issuer" } })
             .Build();
         var logger = Substitute.For<ILogger>();
         var configParams = new ConfigurationParams(config);

         Assert.Throws<InvalidOperationException>(() =>
             configParams.ValidateConfiguration(logger, "Jwt:Key", "Jwt:Issuer", "Jwt:Audience"));
     }
     ```

5. **Global Exception Handler**:
   - Your global exception handler won’t interfere with startup validation because it’s configured after `app.Build()`. If you want to log startup errors differently, configure a logger in `Program.cs`:
     ```csharp
     builder.Services.AddLogging(logging => logging.AddConsole());
     ```

### Conclusion
The `IConfigurationParams` interface and `ConfigurationParams` class provide a clean, singleton-based solution for managing and validating configuration settings. Place them in `ESLAdmin.Infrastructure.Configuration` for alignment with your infrastructure layer. Register and validate the singleton in `Program.cs` to fail fast on missing configurations, bypassing the global exception handler. Inject `IConfigurationParams` into command handlers like `RefreshTokenCommandHandler` for safe access to validated settings. The solution is efficient, type-safe, and reusable across your application.

If you share the full `RefreshTokenCommandHandler` or additional configuration keys, I can refine the implementation further. Let me know if you need help with testing, additional features, or integration with other components!

<xaiArtifact artifact_id="2bbe2540-6031-4482-93ce-8ba5eed0751f" artifact_version_id="1c84a8ff-6b90-4b52-9ef4-20eabf61553e" title="IConfigurationParams.cs" contentType="text/plain">

namespace ESLAdmin.Infrastructure.Configuration;

public interface IConfigurationParams
{
    IReadOnlyDictionary<string, string> Settings { get; }
    void ValidateConfiguration(ILogger logger, params string[] requiredKeys);
}

</xaiArtifact>

<xaiArtifact artifact_id="e2ba9081-2c24-44a8-94cc-510aa68902ad" artifact_version_id="3f4fcbbd-3aac-43b1-a00e-eb16af5e9e8f" title="ConfigurationParams.cs" contentType="text/plain">

namespace ESLAdmin.Infrastructure.Configuration;

public class ConfigurationParams : IConfigurationParams
{
    private readonly Dictionary<string, string> _settings;

    public IReadOnlyDictionary<string, string> Settings => _settings;

    public ConfigurationParams(IConfiguration config)
    {
        _settings = new Dictionary<string, string>
        {
            { "Jwt:Key", config["Jwt:Key"] ?? "" },
            { "Jwt:Issuer", config["Jwt:Issuer"] ?? "" },
            { "Jwt:Audience", config["Jwt:Audience"] ?? "" }
        };
    }

    public void ValidateConfiguration(ILogger logger, params string[] requiredKeys)
    {
        var missingKeys = requiredKeys
            .Where(key => !_settings.ContainsKey(key) || string.IsNullOrEmpty(_settings[key]))
            .ToList();

        if (missingKeys.Any())
        {
            foreach (var key in missingKeys)
            {
                logger.LogError("{ConfigKey} is not found or empty in the configuration", key);
            }
            throw new InvalidOperationException(
                $"Missing or empty configuration keys: {string.Join(", ", missingKeys)}");
        }
    }
}

</xaiArtifact>