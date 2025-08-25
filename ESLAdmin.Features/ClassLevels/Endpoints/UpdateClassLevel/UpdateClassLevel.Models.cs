namespace ESLAdmin.Features.ClassLevels.Endpoints.UpdateClassLevel;

//------------------------------------------------------------------------------
//
//                           class UpdateClassLevelRequest
//
//------------------------------------------------------------------------------
public class UpdateClassLevelRequest
{
  public long ClassLevelId { get; set; }
  public string ClassLevelName { get; set; } = string.Empty;
  public int DisplayOrder { get; set; } = 0;
  public int DisplayColor { get; set; } = 0;
  public long UserCode { get; set; }
  public string Guid { get; set; } = string.Empty;
}

//------------------------------------------------------------------------------
//
//                           class UpdateClassLevelResponse
//
//------------------------------------------------------------------------------
public class UpdateClassLevelResponse
{
  public long ClassLevelId { get; set; }
  public string Guid { get; set; } = string.Empty;
}
