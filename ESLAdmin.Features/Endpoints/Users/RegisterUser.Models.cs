using FastEndpoints;
using FluentValidation;

namespace ESLAdmin.Features.Endpoints.Users;

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
            .NotEmpty().WithMessage("Email is required.");
  }
}