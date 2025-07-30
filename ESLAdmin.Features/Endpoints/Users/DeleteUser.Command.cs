using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESLAdmin.Features.Endpoints.Users
{
  public class DeleteUserCommand : ICommand<Results<Ok<string>, ProblemDetails, InternalServerError>>
  {
    public string Email { get; set; } = string.Empty;
  }
}
