using FastEndpoints;
using FluentValidation;

namespace ESLAdmin.Features.ClassLevels.Endpoints.CreateClassLevel;

//------------------------------------------------------------------------------
//
//                           class CreateClassLevelRequest
//
//------------------------------------------------------------------------------
public class CreateClassLevelRequest
{
  public string ClassLevelName { get; set; } = string.Empty;
  public int DisplayOrder { get; set; }
  public int DisplayColor { get; set; }
  public long InitUser { get; set; }
}

//------------------------------------------------------------------------------
//
//                           class CreateClassLevelResponse
//
//------------------------------------------------------------------------------
public class CreateClassLevelResponse
{
  public long ClassLevelId { get; set; }
  public required string Guid { get; set; }
}

//------------------------------------------------------------------------------
//
//                           class CreateClassLevelValidator
//
//------------------------------------------------------------------------------
public class CreateClassLevelValidator : Validator<CreateClassLevelRequest>
{
  public CreateClassLevelValidator()
  {
    RuleFor(x => x.ClassLevelName)
      .NotEmpty().WithMessage("Class level name is required.");
    RuleFor(x => x.DisplayOrder)
      .GreaterThan(0).WithMessage("Display order must be greater than 0.");
    RuleFor(x => x.DisplayColor)
      .GreaterThan(0).WithMessage("Display color must be greater than 0.");
  }
}
