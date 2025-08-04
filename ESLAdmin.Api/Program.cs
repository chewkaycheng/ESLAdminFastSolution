global using FastEndpoints;
using ESLAdmin.Api.Extensions;
using ESLAdmin.Features.Extensions;
using ESLAdmin.Logging.Extensions;
using FastEndpoints.Swagger;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureLogging(builder);
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureIdentity(builder.Configuration);
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
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    In = ParameterLocation.Header,
    Description = "Please enter JWT with Bearer into field",
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer"
  });
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
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
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.Run();
