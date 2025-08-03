using FastEndpoints;
using FluentValidation;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                       class GetUserRequest
//
//-------------------------------------------------------------------------------
public class GetUserRequest
{
  public string Email { get; set; } = string.Empty;
}

//------------------------------------------------------------------------------
//
//                       class GetUserResponse
//
//-------------------------------------------------------------------------------
public class GetUserResponse
{
  public required string Id { get; init; }
  public string? FirstName { get; init; }
  public string? LastName { get; init; }
  public string? UserName { get; init; }
  public string? Email { get; init; }
  public string? PhoneNumber { get; init; }
  public ICollection<string>? Roles { get; init; }
}

//------------------------------------------------------------------------------
//
//                        class GetUserValidator
//
//-------------------------------------------------------------------------------
public class GetUserValidator : Validator<GetUserRequest>
{
  public GetUserValidator()
  {
    RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");
  }
}