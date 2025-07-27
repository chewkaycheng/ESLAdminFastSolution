namespace ESLAdmin.Domain.Entities;

//------------------------------------------------------------------------------
//
//                        Class ClassLevel
//
//------------------------------------------------------------------------------
public class ClassLevel : EntityBase
{
  public long ClassLevelId { get; set; } = 0;
  public int DisplayOrder { get; set; } = 0;
  public int DisplayColor { get; set; } = 0;
}
