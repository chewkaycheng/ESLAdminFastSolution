using FastEndpoints;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                           class CreateChildcareLevelRequest
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelRequest
{
  public string ChildcareLevelName { get; set; } = string.Empty;
  public int MaxCapacity { get; set; }
  public int DisplayOrder { get; set; }
  public long InitUser { get; set; }
}

//------------------------------------------------------------------------------
//
//                           class CreateChildcareLevelValidator
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelValidator : Validator<CreateChildcareLevelRequest>
{
  public CreateChildcareLevelValidator()
  {

  }
}