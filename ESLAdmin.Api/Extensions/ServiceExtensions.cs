using ESLAdmin.Common.Configuration;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using ESLAdmin.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Infrastructure.Services;
using ESLAdmin.Logging;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text;

namespace ESLAdmin.Api.Extensions;

public static class ServiceExtensions
{
  // =================================================
  // 
  // ConfigureCors
  //
  // ==================================================
  public static void ConfigureCors(
      this IServiceCollection services) =>
      services.AddCors(options =>
      {
        options.AddPolicy(
          "CorsPolicy",
          builder =>
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
      });

  // =================================================
  // 
  // ConfigureIISIntegration
  //
  // ==================================================
  public static void ConfigureIISIntegration(
      this IServiceCollection services) =>
        services.Configure<IISOptions>(options =>
        {

        });

  // =================================================
  // 
  // ConfigureIdentity
  //
  // ==================================================
  public static void ConfigureIdentity(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    var passwordLength = configuration
      .GetValue<int>("Identity:Password:RequiredLength", 5);

    var builder =
      services.AddIdentity<User, IdentityRole>(o =>
      {
        o.Password.RequireDigit = true;
        o.Password.RequireLowercase = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequiredLength = passwordLength;
        o.User.RequireUniqueEmail = true;
      })
      .AddEntityFrameworkStores<UserDbContext>()
      .AddDefaultTokenProviders();
  }

  // =================================================
  // 
  // ConfigureFastEndpoints
  //
  // ==================================================
  public static void ConfigureFastEndpoints(
   this IServiceCollection services)
  {
    services.AddFastEndpoints(options =>
    {
      options.Assemblies = new[] { typeof(
        ESLAdmin.Features.FeatureAssemblyMarker).Assembly};
    });
  }

  // =================================================
  // 
  // ConfigureFirebirdDbContexts
  //
  // ==================================================
  public static void ConfigureFirebirdDbContexts(
   this IServiceCollection services,
   IConfiguration configuration)
  {
    services.AddDbContext<UserDbContext>(opts =>
        opts.UseFirebird(
          configuration.GetConnectionString("ESLAdminConnection"), b=>b.MigrationsAssembly("ESLAdmin.Api")));

    services.AddSingleton<IDbContextDapper, DbContextDapper>();
    services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
  }

  // =================================================
  // 
  // ConfigureRepositoryManager
  //
  // ==================================================
  public static void ConfigureRepositoryManager(
    this IServiceCollection services)
  {
    services.AddScoped<IRepositoryManager, RepositoryManager>();
  }

  // =================================================
  // 
  // ConfigureBlacklistService
  //
  // ==================================================
  public static void ConfigureBlacklistService(
    this IServiceCollection services)
  {
    services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
  }

  // =================================================
  // 
  // ConfigureSwagger
  //
  // ==================================================
  public static IServiceCollection ConfigureSwagger(
    this IServiceCollection services)
  {
    services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo
      {
        Title = "ESLAdmin API",
        Version = "v1"
      });
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
          Array.Empty<string>()
        }
      });
    });
    return services;
  }

  // =================================================
  // 
  // ConfigureExceptionHandler
  //
  // ==================================================
  public static void ConfigureExceptionHandler(this IApplicationBuilder app)
  {
    app.Run(async context =>
    {
      var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

      var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
      if (exception != null)
      {
        logger.LogException(exception);

        await Results.Problem(
          title: "An internal server error has occurred. Please try again later.",
          statusCode: StatusCodes.Status500InternalServerError,
          extensions: new Dictionary<string, object?>
          {
            {"traceId", Activity.Current?.Id}
          }).ExecuteAsync(context);
      }
    });
  }

  // =================================================
  // 
  // ConfigureJwtExtensions
  //
  // ==================================================
  public static void ConfigureJwtExtensions(
    this IServiceCollection services,
    IConfigurationParams configParams)
  {
    var configKeys = configParams.Settings;
    var jwtKey = configKeys["Jwt:Key"];
    var issuer = configKeys["Jwt:Issuer"];
    var audience = configKeys["Jwt:Audience"];

    services
      .AddAuthenticationJwtBearer(
        signingOptions: s => s.SigningKey = jwtKey,
        bearerOptions: o =>
        {
          o.ClaimsIssuer = issuer;
          o.Audience = audience;
          o.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
          };
          o.Events = new()
          {
            OnChallenge = async ctx =>
            {
              ctx.HandleResponse();
              var requiresAuth =
                ctx.HttpContext
                   .GetEndpoint()
                   .Metadata.OfType<IAuthorizeData>()
                   .Any();          
              if (ctx.AuthenticateFailure is not null || requiresAuth is true)
              {
                await ctx.Response.SendErrorsAsync(
                  [new("Security", "You are not authorized.")], 401);
              }
            }
          };
        })
      .AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme =
          JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme =
          JwtBearerDefaults.AuthenticationScheme;
      });
  }
}
