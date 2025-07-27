using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users;

public class LoginUserCommand : 
    ICommand<Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}