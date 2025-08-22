using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ESLAdmin.Common.Configuration;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ESLAdmin.Features.IdentityUsers.RefreshJwtToken;

public class RefreshTokenJwtCommandHandler : ICommandHandler<RefreshJwtTokenCommand,
  Results<Ok<RefreshJwtTokenResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<RefreshTokenJwtCommandHandler> _logger;
  private readonly ApiSettings _apiSettings;

  public RefreshTokenJwtCommandHandler(
    IRepositoryManager repositoryManager,
    IOptions<ApiSettings> settings,
    ILogger<RefreshTokenJwtCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _apiSettings = settings.Value;
    _logger = logger;
  }

  //private ProblemDetails InvalidTokenError()
  //{
  //  return new ProblemDetails(
  //    ErrorUtils.CreateFailureList(
  //      "InvalidToken",
  //      "The provided token is invalid."),
  //    StatusCodes.Status400BadRequest);
  //}

  public async Task<Results<Ok<RefreshJwtTokenResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(RefreshJwtTokenCommand command, CancellationToken ct)
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
        return error.Code switch
        {
          "Database.OperationCanceled" => AppErrors.ProblemDetailsFactory.RequestTimeout(),
          string code when code.Contains("Exception") => TypedResults.InternalServerError(),
          _ => AppErrors.ProblemDetailsFactory.TokenError()
        };
      }

      var refreshToken = result.Value;
      var jwtSettings = _apiSettings.Jwt;
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtSettings.Key));

      // Return token error if validation fails
      tokenHandler.ValidateToken(
        command.AccessToken,
        new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = false, // Allow expired token
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings.Issuer,
          ValidAudience = jwtSettings.Audience,
          IssuerSigningKey = key
        },
        out SecurityToken validatedToken);

      var jwtToken = (JwtSecurityToken)validatedToken;
      var userId = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
      if (userId == null)
        return AppErrors.ProblemDetailsFactory.TokenError();

      // Verify user
      var userResult = await _repositoryManager
        .IdentityRepository
        .FindByIdAsync(userId);

      if (userResult.IsError)
      {
        var error = userResult.Errors.First();
        return error.Code switch
        {
          "Database.OperationCanceled" => AppErrors.ProblemDetailsFactory.RequestTimeout(),
          string code when code.Contains("Exception") => TypedResults.InternalServerError(),
          _ => AppErrors.ProblemDetailsFactory.TokenError()
        };
      }

      var user = userResult.Value;
      if (user.Id != refreshToken.UserId)
      {
        return AppErrors.ProblemDetailsFactory.TokenError();
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
          jwtSettings.Key));
      var creds = new SigningCredentials(
        newKey, SecurityAlgorithms.HmacSha256);
      var newToken = new JwtSecurityToken(
        issuer: jwtSettings.Issuer,
        audience: jwtSettings.Audience,
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds);

      var newAccessToken = new JwtSecurityTokenHandler().WriteToken(newToken);

      // Generate new refresh token
      var newRefreshToken = new Infrastructure.Persistence.Entities.RefreshToken
      {
        UserId = user.Id,
        Token = Guid.NewGuid().ToString(),
        IssuedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        IsRevoked = false
      };

      var tokenResult = await _repositoryManager
        .IdentityRepository
        .ReplaceRefreshTokenAsync(newRefreshToken, command.RefreshToken);

      if (tokenResult.IsError)
      {
        var error = tokenResult.Errors.FirstOrDefault();
        return error.Code switch
        {
          "Database.ConcurrencyFailure" => AppErrors.ProblemDetailsFactory.ConcurrencyFailure(),
          "Database.OperationCanceled" => AppErrors.ProblemDetailsFactory.RequestTimeout(),
          string code when code.Contains("Exception") => TypedResults.InternalServerError(),
          _ => AppErrors.ProblemDetailsFactory.TokenError()
        };
      }

      //var refreshTokenResult = await _repositoryManager
      //  .IdentityRepository
      //  .AddRefreshTokenAsync(newRefreshToken);

      //if (refreshTokenResult.IsError)
      //  return TypedResults.InternalServerError();

      //// Revoke old refresh token
      //var tokenResult = await _repositoryManager
      //  .IdentityRepository
      //  .RevokeRefreshTokenAsync(command.RefreshToken);

      //if (tokenResult.IsError)
      //{
      //  var error = userResult.Errors.First();
      //  return error.Code switch
      //  {
      //    "Database.ConcurrencyFailure" => AppErrors.CustomProblemDetails.ConcurrencyFailure(),
      //    "Database.OperationCanceled" => AppErrors.CustomProblemDetails.RequestTimeout(),
      //    string code when code.Contains("Exception") => TypedResults.InternalServerError()
      //    _ => AppErrors.CustomProblemDetails.TokenError()
      //  };
      //}

      return TypedResults.Ok(new RefreshJwtTokenResponse
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
        SecurityTokenInvalidSignatureException tokenException => 
          AppErrors.ProblemDetailsFactory.TokenError(),
        SecurityTokenInvalidIssuerException tokenException => 
          AppErrors.ProblemDetailsFactory.TokenError(),
        SecurityTokenInvalidAudienceException tokenException => 
          AppErrors.ProblemDetailsFactory.TokenError(),
        SecurityTokenMalformedException tokenException => 
          AppErrors.ProblemDetailsFactory.TokenError(),
        SecurityTokenValidationException tokenException => 
          AppErrors.ProblemDetailsFactory.TokenError(),
        SecurityTokenException tokenException => 
          AppErrors.ProblemDetailsFactory.TokenError(),
        _ => TypedResults.InternalServerError()
      };
    }
  }
}
