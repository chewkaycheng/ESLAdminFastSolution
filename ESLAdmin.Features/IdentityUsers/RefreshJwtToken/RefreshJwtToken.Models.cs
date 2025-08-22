namespace ESLAdmin.Features.IdentityUsers.RefreshJwtToken;

//-------------------------------------------------------------------------------
//
//                       class RefreshTokenRequest
//
//-------------------------------------------------------------------------------
public class RefreshJwtTokenRequest
{
  public string AccessToken { get; set; }
  public string RefreshToken { get; set; } 
}

//-------------------------------------------------------------------------------
//
//                       class RefreshTokenResponse
//
//-------------------------------------------------------------------------------
public class RefreshJwtTokenResponse
{
  public string AccessToken { get; set; }
  public string RefreshToken { get; set; }
  public DateTime Expires { get; set; }
}

