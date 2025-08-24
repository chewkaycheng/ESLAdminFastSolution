namespace ESLAdmin.Features.ClassLevels.Infrastructure.Persistence.Constants;

public class DbConstsClassLevel
{
  public const string PARAM_CLASSLEVELID = "pclasslevelid";
  public const string PARAM_CLASSLEVELNAME = "pclassLevelName";
  public const string PARAM_DISPLAYORDER = "pdisplayorder";
  public const string PARAM_DISPLAYCOLOR = "pdisplaycolor";
  public const string PARAM_INITUSER = "pinituser";
  public const string PARAM_USERCODE = "pusercode";
  public const string PARAM_GUID = "pguid";
  
  public const string CLASSLEVELID = "classlevelid";
  public const string CLASSLEVELNAME = "classlevelname";
  public const string DISPLAYORDER = "displayorder";
  public const string DISPLAYCOLOR = "displaycolor";

  public const string SQL_GETALL =
    @"select 
        CLASSLEVELID as ID,
        CLASSLEVELNAME,
        DISPLAYORDER,
        DISPLAYCOLOR,
        INITUSER,
        INITDATE,
        USERCODE,
        USERSTAMP,
        GUID
      from
        CLASSLEVELS 
      order by
        CLASSLEVELNAME
      ";
  
  public const string SQL_GETBYID =
    @"select 
        CLASSLEVELID as ID,
        CLASSLEVELNAME,
        DISPLAYORDER,
        DISPLAYCOLOR,
        INITUSER,
        INITDATE,
        USERCODE,
        USERSTAMP,
        GUID
      from
        CLASSLEVELS 
      where
        CLASSLEVELID = @classlevelid
      order by
        CLASSLEVELNAME
      ";
  public const string SP_CLASSLEVEL_ADD = "classlevel_add";
  public const string SP_CLASSLEVEL_UPD = "classlevel_upd";
  public const string SP_CLASSLEVEL_DEL =
    "classlevel_del";
}

