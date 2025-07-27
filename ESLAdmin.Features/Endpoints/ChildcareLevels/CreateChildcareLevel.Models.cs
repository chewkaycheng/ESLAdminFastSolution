using FastEndpoints;
using FluentValidation;

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
//                           class CreateChildcareLevelResponse
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelResponse
{
  public long ChildcareLevelId { get; set; }
  public string Guid { get; set; }
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
    RuleFor(x => x.ChildcareLevelName)
            .NotEmpty().WithMessage("Childcare level is required.");
  }
}