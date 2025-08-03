using FastEndpoints;
using FluentValidation;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        class AddToRoleRequest
//
//-------------------------------------------------------------------------------
public class AddToRoleRequest
{
  public string Email { get; set; } = string.Empty;
  public string RoleName { get; set; } = string.Empty;
}

//------------------------------------------------------------------------------
//
//                        class AddToRoleValidator
//
//-------------------------------------------------------------------------------
public class AddToRoleValidator : Validator<AddToRoleRequest>
{
  public AddToRoleValidator()
  {
    RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");
    RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role name is required.");
  }
}