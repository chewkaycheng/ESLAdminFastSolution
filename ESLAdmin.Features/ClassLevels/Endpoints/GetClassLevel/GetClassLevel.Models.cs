namespace ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevel;

//------------------------------------------------------------------------------
//
//                        class GetClassLevelRequest 
//
//------------------------------------------------------------------------------
public class GetClassLevelRequest
{
  public long Id { get; set; }
}

//------------------------------------------------------------------------------
//
//                        class GetClassLevelResponse 
//
//------------------------------------------------------------------------------
public class GetClassLevelResponse
{
  public long Id { get; set; }
  public string ClassLevelName { get; set; } = string.Empty;  
  public int DisplayOrder { get; set; }
  public int DisplayColor { get; set; }
  public long InitUser { get; set; }
  public DateTime InitDate { get; set; }
  public long UserCode { get; set; }
  public DateTime UserStamp { get; set; }
  public string Guid { get; set; } = string.Empty;
}