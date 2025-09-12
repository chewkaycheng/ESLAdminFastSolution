namespace ESLAdmin.Infrastructure.Persistence.Entities;

//------------------------------------------------------------------------------
//
//                        Class ClassLevel
//
//------------------------------------------------------------------------------
public class ClassLevel : EntityBase
{
  public long ClassLevelId { get; set; } = 0;
  public string ClassLevelName { get; set; } = string.Empty; 
  public int DisplayOrder { get; set; } = 0;
  public int DisplayColor { get; set; } = 0;
}
