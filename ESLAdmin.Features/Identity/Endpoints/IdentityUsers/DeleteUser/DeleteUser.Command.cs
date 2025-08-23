using ErrorOr;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Identity.Endpoints.IdentityUsers.DeleteUser
{
  public class DeleteUserCommand : ICommand<Results<Ok<Success>, ProblemDetails, InternalServerError>>
  {
    public string Email { get; set; } = string.Empty;
  }
}
