using FastEndpoints;
using FluentValidation;

namespace ESLAdmin.Features.Countries.CreateCountry;

//------------------------------------------------------------------------------
//
//                           class CreateCountryRequest
//
//------------------------------------------------------------------------------
public class CreateCountryRequest
{
  public string CountryName { get; set; } = string.Empty;
  public string LanguageName {  get; set; } = string.Empty;
  public long InitUser {  get; set; }
}

//------------------------------------------------------------------------------
//
//                           class CreateCountryResponse
//
//------------------------------------------------------------------------------
public class CreateCountryResponse
{
  public long CountryId { get; set; }
  public required string Guid { get; set; }
}

//------------------------------------------------------------------------------
//
//                           class CreateCountryValidator
//
//------------------------------------------------------------------------------
public class CreateCountryValidator : Validator<CreateCountryRequest>
{
  public CreateCountryValidator()
  {
    RuleFor(x => x.CountryName)
      .NotEmpty().WithMessage("Country name is required.");
    RuleFor(x => x.LanguageName)
     .NotEmpty().WithMessage("Language name is required.");
  }
}
