using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.UpdateChildcareLevel;

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelRequest
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelRequest
{
  public long ChildcareLevelId { get; set; }
  public string ChildcareLevelName { get; set; } = string.Empty;
  public int MaxCapacity { get; set; }
  public int DisplayOrder { get; set; }
  public long UserCode { get; set; }
  public string Guid { get; set; } = string.Empty;
}

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelValidator
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelValidator : Validator<UpdateChildcareLevelRequest>
{
  public UpdateChildcareLevelValidator()
  {

  }
}
