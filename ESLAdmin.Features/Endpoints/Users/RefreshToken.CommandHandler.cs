using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
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
  private readonly IConfiguration _configuration;
  private readonly ILogger<RefreshTokenCommandHandler> _logger;

  public RefreshTokenCommandHandler(
    IRepositoryManager repositoryManager,
    IConfiguration configuration,
    ILogger<RefreshTokenCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _configuration = configuration;
    _logger = logger;
  }
  public async Task<Results<Ok<RefreshTokenResponse>, ProblemDetails, InternalServerError>> 
    ExecuteAsync(RefreshTokenCommand command, CancellationToken ct)
  {
    try
    {
      // Validate refresh token
      var result = await _repositoryManager
                                  .AuthenticationRepository
                                  .GetRefreshTokenAsync(command.RefreshToken);
      if (result.IsError)
      {
        foreach (var error in result.Errors)
        {
          if (error.Code == "Exception")
            return TypedResults.InternalServerError();

          var validationFailures = new List<FluentValidation.Results.ValidationFailure>();
          validationFailures.AddRange(new FluentValidation.Results.ValidationFailure
          {
            PropertyName = "Invalid refresh token",
            ErrorMessage = "The provided refresh token is invalid or has expired."
          });
          return new ProblemDetails(validationFailures, StatusCodes.Status400BadRequest);
        }
      }

      var refreshToken = result.Value;
      // Validate acces token (allow expired token)
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
      tokenHandler.ValidateToken(command.AccessToken, new TokenValidationParameters 
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false, // Allow expired token
        ValidateIssuerSigningKey = true,
        ValidIssuer = _configuration["Jwt:Issuer"],
        ValidAudience = _configuration["Jwt:Audience"],
        IssuerSigningKey = key
      }, out SecurityToken validatedToken);

      var jwtToken = (JwtSecurityToken)validatedToken;
      var userId = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

      // Verify user
      var userResult = await _repositoryManager
        .AuthenticationRepository
        .FindByIdAsync(userId);
      if (userResult.IsError)
      {
        foreach (var error in userResult.Errors)
        {
          if (error.Code == "Exception")
            return TypedResults.InternalServerError();

          var validationFailures = new List<FluentValidation.Results.ValidationFailure>();
          validationFailures.AddRange(new FluentValidation.Results.ValidationFailure
          {
            PropertyName = "User not found",
            ErrorMessage = $"The user with Id: '{userId}' is not found."
          });
          return new ProblemDetails(validationFailures, StatusCodes.Status404NotFound);
        }
      }

      var user = userResult.Value;
      if (user.Id != refreshToken.UserId)
      {
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>();
        validationFailures.AddRange(new FluentValidation.Results.ValidationFailure
        {
          PropertyName = "Invalid token",
          ErrorMessage = "The access token does not match the refresh token."
        });
        return new ProblemDetails(validationFailures, StatusCodes.Status400BadRequest);
      }

      // Revoke old refresh token
      var tokenResult = await _repositoryManager
        .AuthenticationRepository
        .RevokeRefreshTokenAsync(command.RefreshToken);
      if (tokenResult.IsError)
      {
        return TypedResults.InternalServerError();
      }
      
      // Generate new access token
      var resultRoles = await _repositoryManager
        .AuthenticationRepository
        .GetRolesAsync(user);
      if (resultRoles.IsError)
      {
        return TypedResults.InternalServerError();
      }
      var userRoles = resultRoles.Value;
      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.UserName)
      };
      claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

      var newKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(
          _configuration["Jwt:Key"]));
      var creds = new SigningCredentials(
        newKey, SecurityAlgorithms.HmacSha256);
      var newToken = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
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
      await _repositoryManager
        .AuthenticationRepository
        .AddRefreshTokenAsync(newRefreshToken);

      return TypedResults.Ok(new RefreshTokenResponse
      {
        AccessToken = newAccessToken,
        RefreshToken = newRefreshToken.Token,
        Expires = newToken.ValidTo
      });
    }
    catch (SecurityTokenException )
    {
      var validationFailures = new List<FluentValidation.Results.ValidationFailure>
      {
        new FluentValidation.Results.ValidationFailure
        {
          PropertyName = "Invalid access token",
          ErrorMessage = "The provided access token is invalid."
        }
      };
      return new ProblemDetails(validationFailures, StatusCodes.Status400BadRequest);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}
