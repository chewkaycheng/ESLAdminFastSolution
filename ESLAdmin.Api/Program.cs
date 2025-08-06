global using FastEndpoints;
using ESLAdmin.Api.Extensions;
using ESLAdmin.Features.Extensions;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Extensions;
using FastEndpoints.Swagger;
using FluentValidation.Results;
using Microsoft.AspNetCore.Diagnostics;
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

app.UseExceptionHandler(errorApp =>
{
  errorApp.Run(async context =>
  {
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    if (exception != null)
    {
      var logger = context
        .RequestServices
        .GetRequiredService<ILogger<Program>>();
      logger.LogException(exception);
      context.Response.StatusCode = 
        StatusCodes.Status500InternalServerError;

      var validationFailures = new List<ValidationFailure>
      {
        new ValidationFailure
        {
          PropertyName = "Internal Server Error",
          ErrorMessage = "An unexpected error occurred. Please try again later."
        }
      };
      var problemDetails = new ProblemDetails(
        validationFailures, 
        StatusCodes.Status500InternalServerError);
      await context.Response.WriteAsJsonAsync(problemDetails);
    }
  });
});

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.Run();
