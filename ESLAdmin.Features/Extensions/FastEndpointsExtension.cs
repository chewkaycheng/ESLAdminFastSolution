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
