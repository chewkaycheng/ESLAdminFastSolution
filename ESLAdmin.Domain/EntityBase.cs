namespace ESLAdmin.Domain
{
  //------------------------------------------------------------------------------
  //
  //                        Class EntityBase
  //
  //------------------------------------------------------------------------------
  public class EntityBase
  {
    public long InitUser { get; set; } = 0;
    public DateTime InitDate { get; set; }
    public long UserCode { get; set; } = 0;
    public DateTime UserStamp { get; set; }
    public string Guid { get; set; } = string.Empty;
  }
}
