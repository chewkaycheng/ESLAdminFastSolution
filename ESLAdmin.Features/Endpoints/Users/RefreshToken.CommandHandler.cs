using ErrorOr;
using ESLAdmin.Common.Configuration;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ESLAdmin.Features.Endpoints.Users;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand,
  Results<Ok<RefreshTokenResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<RefreshTokenCommandHandler> _logger;
  private readonly IConfigurationParams _configurationParams;

  public RefreshTokenCommandHandler(
    IRepositoryManager repositoryManager,
    IConfigurationParams configurationParams,
    ILogger<RefreshTokenCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _configurationParams = configurationParams;
    _logger = logger;
  }

  private ProblemDetails InvalidTokenError()
  {
    return new ProblemDetails(
      ErrorUtils.CreateFailureList(
        "Identity.InvalidToken",
        "The provided token is invalid."),
      StatusCodes.Status400BadRequest);
  }

  public async Task<Results<Ok<RefreshTokenResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(RefreshTokenCommand command, CancellationToken ct)
  {
    try
    {
      // Validate refresh token
      var result = await _repositoryManager
                          .IdentityRepository
                          .GetRefreshTokenAsync(command.RefreshToken);
      if (result.IsError)
      {
        var error = result.Errors.First();
        if (error.Code == "Identity.RefreshTokenNotFound")
        {
          return InvalidTokenError();
        }
        return TypedResults.InternalServerError();
      }

      var refreshToken = result.Value;
      var settings = _configurationParams.Settings;
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(settings["Jwt:Key"]));

      tokenHandler.ValidateToken(
        command.AccessToken,
        new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = false, // Allow expired token
          ValidateIssuerSigningKey = true,
          ValidIssuer = settings["Jwt:Issuer"],
          ValidAudience = settings["Jwt:Audience"],
          IssuerSigningKey = key
        },
        out SecurityToken validatedToken);

      var jwtToken = (JwtSecurityToken)validatedToken;
      var userId = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

      // Verify user
      var userResult = await _repositoryManager
        .IdentityRepository
        .FindByIdAsync(userId);
      if (userResult.IsError)
      {
        var error = userResult.Errors.First();
        if (error.Code == "Identity.NotFound")
        {
          return InvalidTokenError();
        }
        return TypedResults.InternalServerError();
      }

      var user = userResult.Value;
      if (user.Id != refreshToken.UserId)
      {
        return InvalidTokenError();
      }

      // Revoke old refresh token
      var tokenResult = await _repositoryManager
        .IdentityRepository
        .RevokeRefreshTokenAsync(command.RefreshToken);
      if (tokenResult.IsError)
      {
        var error = userResult.Errors.First();
        if (error.Code == "Identity.InvalidToken")
        {
          return InvalidTokenError();
        }
        return TypedResults.InternalServerError();
      }

      // Generate new access token
      var rolesResult = await _repositoryManager
        .IdentityRepository
        .GetRolesAsync(user);

      if (rolesResult.IsError)
        return TypedResults.InternalServerError();

      var roles = rolesResult.Value;
      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.UserName ?? "")
      };
      claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

      var newKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(
          settings["Jwt:Key"]));
      var creds = new SigningCredentials(
        newKey, SecurityAlgorithms.HmacSha256);
      var newToken = new JwtSecurityToken(
        issuer: settings["Jwt:Issuer"],
        audience: settings["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds);

      var newAccessToken = new JwtSecurityTokenHandler().WriteToken(newToken);

      // Generate new refresh token
      var newRefreshToken = new RefreshToken
      {
        UserId = user.Id,
        Token = Guid.NewGuid().ToString(),
        IssuedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        IsRevoked = false
      };

      var refreshTokenResult = await _repositoryManager
        .IdentityRepository
        .AddRefreshTokenAsync(newRefreshToken);

      if (refreshTokenResult.IsError)
        return TypedResults.InternalServerError();

      return TypedResults.Ok(new RefreshTokenResponse
      {
        AccessToken = newAccessToken,
        RefreshToken = newRefreshToken.Token,
        Expires = newToken.ValidTo
      });
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return ex switch
      {
        SecurityTokenInvalidSignatureException secex => InvalidTokenError(),
        SecurityTokenInvalidIssuerException secex => InvalidTokenError(),
        SecurityTokenInvalidAudienceException secex => InvalidTokenError(),
        SecurityTokenMalformedException secex => InvalidTokenError(),
        SecurityTokenValidationException secex => InvalidTokenError(),
        _ => TypedResults.InternalServerError()
      };
    }
  }
}
