namespace ESLAdmin.Infrastructure.Data.Consts;

//------------------------------------------------------------------------------
//
//                       static class DbConstsChildcareLevel
//
//------------------------------------------------------------------------------
public static class DbConstsChildcareLevel
{
  public const string PARAM_CHILDCARELEVELID = "pchildcarelevelid";
  public const string PARAM_CHILDCARELEVELNAME = "pchildcareLevelName";
  public const string PARAM_MAXCAPACITY = "pmaxcapacity";
  public const string PARAM_DISPLAYORDER = "pdisplayorder";
  public const string PARAM_INITUSER = "pinituser";
  public const string PARAM_USERCODE = "pusercode";
  public const string PARAM_GUID = "pguid";

  public const string CHILDCARELEVELID = "childcarelevelid";
  public const string CHILDCARELEVELNAME = "childcarelevelname";
  public const string MAXCAPACITY = "maxcapacity";
  public const string PLACESASSIGNED = "placesassigned";
  public const string DISPLAYORDER = "displayorder";

  public const string SQL_GETALL =
    @"select 
        CHILDCARELEVELID as ID,
        CHILDCARELEVELNAME,
        MAXCAPACITY,
        PLACESASSIGNED,
        DISPLAYORDER,
        INITUSER,
        INITDATE,
        USERCODE,
        USERSTAMP,
        GUID      
      from
        CHILDCARELEVELS 
      order by
        CHILDCARELEVELNAME
      ";

  public const string SQL_GETBYID =
    @"select 
        CHILDCARELEVELID as ID,
        CHILDCARELEVELNAME,
        MAXCAPACITY,
        PLACESASSIGNED,
        DISPLAYORDER,
        INITUSER,
        INITDATE,
        USERCODE,
        USERSTAMP,
        GUID      
      from
        CHILDCARELEVELS 
      where
        CHILDCARELEVELID = @id 
      order by
        CHILDCARELEVELNAME 
      ";
  public const string SP_CHILDCARELEVEL_ADD = "childcarelevel_add";
  public const string SP_CHILDCARELEVEL_UPD = "childcarelevel_upd";
  public const string SP_CHILDCARELEVEL_DEL = 
    "childcarelevel_del";
}
