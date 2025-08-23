namespace ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevel;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelRequest 
//
//------------------------------------------------------------------------------
public class GetChildcareLevelRequest
{
  public long Id { get; set; }
}

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelResponse 
//
//------------------------------------------------------------------------------
public class GetChildcareLevelResponse
{
  public long Id { get; set; }
  public string ChildcareLevelName { get; set; } = string.Empty;
  public int MaxCapacity { get; set; }
  public int DisplayOrder { get; set; }
  public int PlacesAssigned { get; set; }
  public long InitUser { get; set; }
  public DateTime InitDate { get; set; }
  public long UserCode { get; set; }
  public DateTime UserStamp { get; set; }
  public string Guid { get; set; } = string.Empty;
}