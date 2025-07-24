using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                       class RegisterUserCommand
//
//-------------------------------------------------------------------------------
public class RegisterUserCommand : ICommand<Results<NoContent, ProblemDetails, InternalServerError>>
{
  public required string UserName { get; init; } 
  public string? FirstName { get; init; } 
  public string? LastName { get; init; } 
  public required string Email { get; init; } 
  public required string Password { get; init; } 
  public string? PhoneNumber { get; init; } 
  public ICollection<string>? Roles { get; init; }
  public required RegisterUserMapper Mapper { get; init; }
}

