using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ESLAdmin.Features.Extensions;

//------------------------------------------------------------------------------
//
//                      static class FastEndpointsExtension
//
//------------------------------------------------------------------------------

public static class FastEndpointsExtension
{
  public static void ConfigureJwtExtensions(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration.");
    var issuer = configuration["Jwt:Issuer"] ?? "YourIssuer";
    var audience = configuration["Jwt:Audience"] ?? "YourAudience";

    services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
      options.ClaimsIssuer = issuer;
      options.Audience = audience;
      options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
      };
    });
  }
}
