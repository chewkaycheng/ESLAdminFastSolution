using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.CreateChildcareLevel;

public class Request
{
  public string ChildcareLevelName { get; set; } = string.Empty;
  public int MaxCapacity { get; set; }
  public int DisplayOrder { get; set; }
  public long InitUser { get; set; }
}

public class Validator : Validator<Request>
{
  public Validator()
  {

  }
}