using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class LoginUserRequest
//
//-------------------------------------------------------------------------------
public class LoginUserRequest
{
  [Required]
  public string Email { get; set; } = string.Empty;

  [Required]
  [DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;

  public bool RememberMe { get; set; }
}

//-------------------------------------------------------------------------------
//
//                      LoginUserResponse
//
//-------------------------------------------------------------------------------
public class LoginUserResponse
{
  public string Token { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
  public string? Email { get; set; } = string.Empty;
}