using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Features.Users.Endpoints.RegisterUser
{
  using ESLAdmin.Features.Exceptions;
  using ESLAdmin.Features.Repositories.Interfaces;
  using ESLAdmin.Logging;
  using ESLAdmin.Logging.Interface;
  using FastEndpoints;
  using FluentValidation.Results;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Http.HttpResults;
  using Microsoft.Extensions.Logging;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  //------------------------------------------------------------------------------
  //
  //                       class RegisterUserCommand
  //
  //-------------------------------------------------------------------------------
  public class RegisterUserCommand : ICommand<Results<NoContent, ProblemDetails, InternalServerError>>
  {
    public string UserName { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public string PhoneNumber { get; init; }
    public ICollection<string>? Roles { get; init; }
    public RegisterUserMapper Mapper { get; init; }
  }

  //------------------------------------------------------------------------------
  //
  //                          RegisterUserRequest
  //
  //-------------------------------------------------------------------------------
  public class RegisterUserRequest
  {
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    [Required(ErrorMessage = "Username is required")]
    public string? UserName { get; init; }
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public ICollection<string>? Roles { get; init; }
  }

  public class RegisterUserResponse
  {
    public string Id { get; init; }
  }
}
