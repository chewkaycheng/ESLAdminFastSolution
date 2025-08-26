namespace ESLAdmin.Features.Countries.Infrastructure.Persistence.Constants;

public class DbConstsCountry
{
  public const string PARAM_COUNTRY_ID = "pcountryid";
  public const string PARAM_COUNTRY_NAME = "pcountryname";
  public const string PARAM_LANGUAGE_NAME = "planguagename";
  public const string PARAM_INITUSER = "pinituser";
  public const string PARAM_USERCODE = "pusercode";
  public const string PARAM_GUID = "pguid";
  
  public const string COUNTRY_ID = "countryid";
  public const string COUNTRY_NAME = "countryname";
  public const string LANGUAGE_NAME = "languagename";

  public const string SQL_GET_ALL =
    @"select 
        COUNTRYID,
        COUNTRYNAME,
        LANGUAGENAME, 
        INITUSER,
        INITDATE,
        USERCODE,
        USERSTAMP,
        GUID    
      from 
        COUNTRIES
      order by 
        COUNTRYNAME";

  public const string SQL_GET_BY_ID =
    @"select 
        COUNTRYID AS ID,
        COUNTRYNAME,
        LANGUAGENAME,
        INITUSER,
        INITDATE,
        USERCODE,
        USERSTAMP,
        GUID    
      from
        COUNTRIES
      where
        COUNTRYID = @countryid
      order by
        COUNTRYNAME";

  public const string SP_COUNTRY_ADD = "country_add";
  public const string SP_COUNTRY_UPD = "country_upd";
  public const string SP_COUNTRY_DEL = "country_del";
}