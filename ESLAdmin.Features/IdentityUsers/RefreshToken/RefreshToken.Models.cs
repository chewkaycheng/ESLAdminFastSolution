namespace ESLAdmin.Features.Endpoints.Users;

//-------------------------------------------------------------------------------
//
//                       class RefreshTokenRequest
//
//-------------------------------------------------------------------------------
public class RefreshTokenRequest
{
  public string AccessToken { get; set; }
  public string RefreshToken { get; set; } 
}

//-------------------------------------------------------------------------------
//
//                       class RefreshTokenResponse
//
//-------------------------------------------------------------------------------
public class RefreshTokenResponse
{
  public string AccessToken { get; set; }
  public string RefreshToken { get; set; }
  public DateTime Expires { get; set; }
}

