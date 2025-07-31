using ESLAdmin.Features.Endpoints.ChildcareLevels;
using FastEndpoints;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Features.Endpoints.Users;

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