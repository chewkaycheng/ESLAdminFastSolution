using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ESLAdmin.Common.Configuration;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Identity.Endpoints.IdentityRoles;
using ESLAdmin.Features.Identity.Entities;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.LoginUser;

//-------------------------------------------------------------------------------
//
//                       class LoginUserCommandHandler
//
//-------------------------------------------------------------------------------
public class LoginUserCommandHandler :
  IdentityCommandHandlerBase<LoginUserCommandHandler>,
  ICommandHandler<LoginUserCommand,
    Results<Ok<LoginUserResponse>, 
      ProblemDetails, 
      InternalServerError>>
{
  private readonly ApiSettings _apiSettings;

  //-------------------------------------------------------------------------------
  //
  //                       LoginUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public LoginUserCommandHandler(
      IIdentityRepository repository,
      ILogger<LoginUserCommandHandler> logger,
      IOptions<ApiSettings> settings) :
    base(repository, logger)
  {
    _apiSettings = settings.Value;
  }

  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
      ExecuteAsync(
          LoginUserCommand command,
          CancellationToken cancellationToken)
  {
    try
    {
      var userResult = await _repository
        .LoginAsync(
          command.Email, command.Password);

      if (userResult.IsError)
      {
        var error = userResult.Errors.First();
        return error.Code switch
        {
          "Database.ConcurrencyFailure" => 
            AppErrors.ProblemDetailsFactory.ConcurrencyFailure(),
          "Database.OperationCanceled" => 
            AppErrors.ProblemDetailsFactory.RequestTimeout(),
          string code when code.Contains("Exception") => 
            TypedResults.InternalServerError(),
          "Identity.IsLockedOut" => 
            AppErrors.ProblemDetailsFactory.LockedOut(),
          "Identity.RequiresTwoFactor" => 
            AppErrors.ProblemDetailsFactory.RequiresTwoFactor(),
          _ => AppErrors.ProblemDetailsFactory.LoginFailed()
        };
      }

      var user = userResult.Value;

      var rolesResult = await _repository
        .GetRolesForUserAsync(user);

      if (rolesResult.IsError)
        return TypedResults.InternalServerError();

      var roles = rolesResult.Value;

      // Generate JWT token
      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.UserName ?? "")
      };
      claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


      //var token = JwtBearer.CreateToken(
      //  o =>
      //  {
      //    o.SigningKey = jwtKey;
      //    o.ExpireAt = DateTime.UtcNow.AddDays(1);
      //    o.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userLoginDto.Id));

      //    if (!string.IsNullOrEmpty(userLoginDto.UserName))
      //    {
      //      o.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Name, userLoginDto.UserName));
      //    }
      //    if (!string.IsNullOrEmpty(userLoginDto.Email))
      //    {
      //      o.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Email, userLoginDto.Email));

      //    }

      //    //o.User["UserId"] = userLoginDto.Id;
      //    if (userLoginDto.Roles != null && userLoginDto.Roles.Any())
      //    {
      //      o.User.Roles.Add(string.Join(",", userLoginDto.Roles));
      //    }
      //    o.ExpireAt = DateTime.UtcNow.AddDays(7);
      //  });

      var jetSettings = _apiSettings.Jwt;
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jetSettings.Key));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken(
          issuer: jetSettings.Issuer,
          audience: jetSettings.Audience,
          claims: claims,
          expires: DateTime.Now.AddHours(1),
          signingCredentials: creds);

      var refreshToken = new RefreshToken
      {
        UserId = user.Id,
        Token = Guid.NewGuid().ToString(),
        IssuedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        IsRevoked = false
      };

      var refreshTokenResult = await _repository
        .AddRefreshTokenAsync(refreshToken);

      if (refreshTokenResult.IsError)
        return TypedResults.InternalServerError();

      LoginUserResponse response = new LoginUserResponse
      {
        UserId = user.Id,
        Email = user.Email,
        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
        RefreshToken = refreshToken.Token,
        Expires = token.ValidTo
      };

      return TypedResults.Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}