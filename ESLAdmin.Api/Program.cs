global using FastEndpoints;
global using FluentValidation;
using ESLAdmin.Api.Extensions;
using ESLAdmin.Logging.Extensions;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureLogging(builder);
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureFirebirdDbContexts(builder.Configuration);
builder.Services.ConfigureRepositoryManagers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddFastEndpoints(options =>
{
  options.Assemblies = new[] { typeof(
    ESLAdmin.Features.FeatureAssemblyMarker).Assembly };
});
builder.Services.AddSwaggerDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

app.UseFastEndpoints();
app.UseSwaggerGen();
app.Run();
