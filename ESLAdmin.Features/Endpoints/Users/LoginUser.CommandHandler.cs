using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class LoginUserCommandHandler
//
//-------------------------------------------------------------------------------
public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand,
    Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<LoginUserCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;
  private readonly IConfiguration _config;

  //-------------------------------------------------------------------------------
  //
  //                       LoginUserCommandHandler
  //
  //-------------------------------------------------------------------------------
  public LoginUserCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<LoginUserCommandHandler> logger,
      IMessageLogger messageLogger,
      IConfiguration config)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _messageLogger = messageLogger;
    _config = config;
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
      var result = await _repositoryManager.AuthenticationRepository.LoginAsync(
          command.Email, command.Password);

      if (result.IsError)
      {
        foreach (var error in result.Errors)
        {
          if (error.Code == "Exception")
            return TypedResults.InternalServerError();

          var validateFailures = new List<FluentValidation.Results.ValidationFailure>();
          validateFailures.AddRange(new FluentValidation.Results.ValidationFailure
          {
            PropertyName = error.Code,
            ErrorMessage = error.Description
          });
          return new ProblemDetails(validateFailures, StatusCodes.Status401Unauthorized);
        }
      }
        
      var userLoginDto = result.Value;

      //Generate JWT token
      var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration.");
      var issuer = _config["Jwt:Issuer"] ?? "YourIssuer";
      var audience = _config["Jwt:Audience"] ?? "YourAudience";

      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, userLoginDto.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, userLoginDto.UserName)
      };
      claims.AddRange(userLoginDto.Roles.Select(role => new Claim(ClaimTypes.Role, role)));


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
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken(
          issuer: _config["Jwt:Issuer"],
          audience: _config["Jwt:Audience"],
          claims: claims,
          expires: DateTime.Now.AddHours(1),
          signingCredentials: creds);

      var refreshToken = new RefreshToken
      {
        UserId = userLoginDto.Id,
        Token = Guid.NewGuid().ToString(),
        IssuedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        IsRevoked = false
      };
      
      var refreshResult = await _repositoryManager
        .AuthenticationRepository
        .AddRefreshTokenAsync(refreshToken);

      if (refreshResult.IsError)
        return TypedResults.InternalServerError();

      LoginUserResponse response = new LoginUserResponse
      {
        UserId = userLoginDto.Id,
        Email = userLoginDto.Email,
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