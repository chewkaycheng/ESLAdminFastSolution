using FastEndpoints;
using FluentValidation;

namespace ESLAdmin.Features.IdentityUsers.DeleteUser;

public class DeleteUserRequest
{
  public string Email { get; set; }
}

public class DeleteUserValidator : Validator<DeleteUserRequest>
{
  public DeleteUserValidator()
  {
    RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");
  }
}