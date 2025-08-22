using FastEndpoints;
using FluentValidation;

namespace ESLAdmin.Features.IdentityUsers.RegisterUser;

//------------------------------------------------------------------------------
//
//                          RegisterUserRequest
//
//-------------------------------------------------------------------------------
public class RegisterUserRequest
{
  public string? FirstName { get; init; }
  public string? LastName { get; init; }
  public required string UserName { get; init; }
  public required string Password { get; init; }
  public required string Email { get; init; }
  public string? PhoneNumber { get; init; }
  public ICollection<string>? Roles { get; init; }
}

//------------------------------------------------------------------------------
//
//                          RegisterUserResponse
//
//-------------------------------------------------------------------------------
public class RegisterUserResponse
{
  public required string Id { get; init; }
}

//------------------------------------------------------------------------------
//
//                          RegisterUserValidator
//
//-------------------------------------------------------------------------------
public class RegisterUserValidator : Validator<RegisterUserRequest>
{
  public RegisterUserValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Invalid email format.")
      .MaximumLength(256).WithMessage("Email cannot exceed 256 characters.");

    RuleFor(x => x.PhoneNumber)
      .NotEmpty().WithMessage("Phone number is required.")
      .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Phone number must be in E.164 format (e.g., +1234567890).")
      .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters."); 

    RuleFor(x => x.UserName)
      .NotEmpty().WithMessage("Username is required.")
      .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
      .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
      .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain letters, numbers, underscores, and hyphens.")
      .Must(username => !new[] { "admin", "system", "root" }.Contains(username, StringComparer.OrdinalIgnoreCase))
      .WithMessage("Username cannot be a reserved word."); ;
  }
}