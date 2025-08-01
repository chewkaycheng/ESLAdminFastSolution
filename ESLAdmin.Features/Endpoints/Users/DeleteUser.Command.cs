using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users
{
  public class DeleteUserCommand : ICommand<Results<Ok<string>, ProblemDetails, InternalServerError>>
  {
    public string Email { get; set; } = string.Empty;
  }
}
