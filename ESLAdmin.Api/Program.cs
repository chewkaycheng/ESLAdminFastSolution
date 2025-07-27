global using FastEndpoints;
using ESLAdmin.Api.Extensions;
using ESLAdmin.Features.Extensions;
using ESLAdmin.Logging.Extensions;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureLogging(builder);
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureFirebirdDbContexts(builder.Configuration);
builder.Services.ConfigureRepositoryManager();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddFastEndpoints(options =>
{
  options.Assemblies = new[] { typeof(
    ESLAdmin.Features.FeatureAssemblyMarker).Assembly};
});

builder.Services.ConfigureJwtExtensions(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.SwaggerDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();
  
app.UseFastEndpoints();
if (app.Environment.IsDevelopment())
{
  app.UseSwaggerGen();
}
app.UseAuthentication();
app.UseAuthorization();
app.Run();
