using ESLAdmin.Common.Configuration;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text;
using ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Repositories;
using ESLAdmin.Features.Identity.Entities;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Contexts;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Features.Identity.Services;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Repositories;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Repositories.Interfaces;

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
    ApiSettings apiSettings)
  {
    var identitySettings = apiSettings.Identity;
    
    var builder =
      services.AddIdentity<User, IdentityRole>(o =>
      {
        o.Password.RequireDigit = identitySettings.Password.RequireDigit;
        o.Password.RequireLowercase = identitySettings.Password.RequireLowercase;
        o.Password.RequireUppercase = identitySettings.Password.RequireUppercase;
        o.Password.RequireNonAlphanumeric = identitySettings.Password.RequireNonAlphanumeric;
        o.Password.RequiredLength = identitySettings.Password.RequiredLength;
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
    //services.AddScoped<IRepositoryManager, RepositoryManager>();
    services.AddScoped<IIdentityRepository, IdentityRepository>();
    services.AddScoped<IChildcareLevelRepository, ChildcareLevelRepository>();
    services.AddScoped<IClassLevelRepository, ClassLevelRepository>();
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
    ApiSettings apiSettings)
  {
    var jwtSettings = apiSettings.Jwt;
    var identitySettings = apiSettings.Identity;
    var jwtKey = jwtSettings.Key;
    var issuer = jwtSettings.Issuer;
    var audience = jwtSettings.Audience;

    services
      .AddAuthenticationJwtBearer(
        signingOptions: s => s.SigningKey = jwtKey,
        bearerOptions: o =>
        {
          o.ClaimsIssuer = issuer;
          o.Audience = audience;
          o.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = (bool)jwtSettings.TokenValidation.ValidateIssuer!,
            ValidateAudience = (bool)jwtSettings.TokenValidation.ValidateAudience!,
            ValidateLifetime = (bool) jwtSettings.TokenValidation.ValidateLifetime!,
            ValidateIssuerSigningKey = (bool) jwtSettings.TokenValidation.ValidateIssuerSigningKey!,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
          };
          o.Events = new()
          {
            OnChallenge = async ctx =>
            {
              ctx.HandleResponse();
              var endpoint = ctx.HttpContext.GetEndpoint();
              var requiresAuth = endpoint?.Metadata.OfType<IAuthorizeData>().Any() ?? false;
                        
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

  // =================================================
  // 
  // ConfigureApiSettings
  //
  // ==================================================
  public static void ConfigureApiSettings(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    services.AddOptions<ApiSettings>()
      .Bind(configuration)
      .ValidateDataAnnotations()
      .ValidateOnStart();
    services.AddSingleton<IValidateOptions<ApiSettings>, ApiSettingsValidator>();
  }

  // =================================================
  // 
  // UseFastEndpointsMiddleware
  //
  // ==================================================
  public static void UseFastEndpointsMiddleware(this IApplicationBuilder app)
  {
    app.UseFastEndpoints(c =>
    {
      c.Errors.UseProblemDetails();
      c.Endpoints.Configurator =
        ep =>
        {
          if (ep.AnonymousVerbs is null)
            ep.Description(b => b.Produces<ProblemDetails>(401));
        };
    });
  }
}
