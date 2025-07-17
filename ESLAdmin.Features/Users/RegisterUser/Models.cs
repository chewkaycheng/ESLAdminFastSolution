using FastEndpoints;
using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Features.Users.RegisterUser;
public class Request
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

public class Response
{
  public int Id { get; set; }
}
public class Validator : Validator<Request>
{
  public Validator()
  {

  }
}