using FastEndpoints;
using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Features.ChildcareLevels.UpdateChildcareLevel;

public class Request
{
  public long ChildcareLevelId { get; set; }
  public string ChildcareLevelName { get; set; } = string.Empty;
  public int MaxCapacity { get; set; }
  public int DisplayOrder { get; set; }
  public long UserCode { get; set; }
  public string Guid { get; set; } = string.Empty;
}
public class Validator : Validator<Request>
{
  public Validator()
  {

  }
}
