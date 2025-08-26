namespace ESLAdmin.Features.Countries.GetCountry;

//------------------------------------------------------------------------------
//
//                        class GetCountryRequest 
//
//------------------------------------------------------------------------------
public class GetCountryRequest
{
  public long Id { get; set; }
}

//------------------------------------------------------------------------------
//
//                        class GetCountryResponse 
//
//------------------------------------------------------------------------------
public class GetCountryResponse
{
  public long Id { get; set; }
  public string CountryName { get; set; }
  public string LanguageName { get; set; }
  public long InitUser { get; set; }
  public DateTime InitDate { get; set; }
  public long UserCode { get; set; }
  public DateTime UserStamp { get; set; }
  public string Guid { get; set; } = string.Empty;
}