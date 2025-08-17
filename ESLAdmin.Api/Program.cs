global using FastEndpoints;
using ESLAdmin.Api.Extensions;
using ESLAdmin.Infrastructure.Configuration;
using ESLAdmin.Infrastructure.Middleware;
using ESLAdmin.Logging.Extensions;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

//using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
//var logger = loggerFactory.CreateLogger<Program>();

//var configParams = new ConfigurationParams(builder.Configuration);
//builder.Services.AddSingleton<IConfigurationParams>(configParams);

//var result = configParams.ValidateConfiguration(logger);
//if (result.IsError)
//{
//  logger.LogError($"Configuration validation failed: {result.Errors.ToFormattedString()}");
//  throw new InvalidOperationException(
//      $"Configuration validation failed: {string.Join("; ", result.Errors.Select(e => e.Description))}");
//}

builder.Services.ConfigureLogging(builder);

var bootstrapper = new ConfigurationBootstrapper(
  builder.Services, 
  builder.Configuration,
  builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>());
var configParams = bootstrapper.Configure();

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureBlacklistService();
builder.Services.ConfigureIdentity(builder.Configuration);
builder.Services.ConfigureFirebirdDbContexts(builder.Configuration);
builder.Services.ConfigureRepositoryManager();

builder.Services.ConfigureFastEndpoints();

builder.Services.ConfigureJwtExtensions(configParams);
builder.Services.AddOpenApi();
builder.Services.SwaggerDocument();
builder.Services.ConfigureSwagger();

// Validate configuration at startup
//var configParams = builder.Services.BuildServiceProvider()
//    .GetRequiredService<IConfigurationParams>();
//var logger = builder.Services.BuildServiceProvider()
//    .GetRequiredService<ILogger<Program>>();

//configParams.ValidateConfiguration(logger);



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

app.UseFastEndpoints();

app.UseExceptionHandler(errorApp =>
  errorApp.ConfigureExceptionHandler()
);

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseMiddleware<JwtBlacklistMiddleware>();
app.UseAuthorization();
app.Run();