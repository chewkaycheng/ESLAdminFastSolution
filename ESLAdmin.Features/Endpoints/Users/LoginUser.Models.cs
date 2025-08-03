using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class LoginUserRequest
//
//-------------------------------------------------------------------------------
public class LoginUserRequest
{
  [Required]
  public string Email { get; set; } = string.Empty;

  [Required]
  [DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;

  public bool RememberMe { get; set; }
}

//-------------------------------------------------------------------------------
//
//                      class LoginUserResponse
//
//-------------------------------------------------------------------------------
public class LoginUserResponse
{
  public string Token { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
  public string? Email { get; set; } = string.Empty;
}

//------------------------------------------------------------------------------
//
//                        class LoginUserValidator
//
//-------------------------------------------------------------------------------
public class LoginUserValidator : Validator<LoginUserRequest>
{
  public LoginUserValidator(IConfiguration configuration)
  {
    var passwordLength = configuration
      .GetValue<int>("Identity:Password:RequiredLength", 5);
    RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");
    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required.")
      .MinimumLength(passwordLength).WithMessage($"Password must be at least {passwordLength} characters long.");
  }
}