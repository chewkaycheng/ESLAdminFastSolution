global using FastEndpoints;
using ESLAdmin.Api.Configuration;
using ESLAdmin.Api.Extensions;
using ESLAdmin.Infrastructure.Extensions;
using ESLAdmin.Infrastructure.Middleware;
using ESLAdmin.Logging.Extensions;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.ConfigureLogging(builder);
var bootstrapper = new ConfigurationBootstrapper(
    builder.Services,
    builder.Configuration);
var configParams = bootstrapper.Configure();

builder.Services.ConfigureJwtExtensions(configParams);
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureFastEndpoints();
builder.Services.ConfigureBlacklistService();
builder.Services.ConfigureIdentity(builder.Configuration);
builder.Services.ConfigureFirebirdDbContexts(builder.Configuration);
builder.Services.ConfigureRepositoryManager();

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


app.UseExceptionHandler(errorApp =>
  errorApp.ConfigureExceptionHandler()
);

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseMiddleware<JwtBlacklistMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpointsMiddleware();

app.Run();