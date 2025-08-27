using ESLAdmin.Features.ClassLevels.Endpoints.CreateClassLevel;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Countries.CreateCountry;

//------------------------------------------------------------------------------
//
//                           class CreateClassLevelCommand
//
//------------------------------------------------------------------------------
public class CreateCountryCommand :
  ICommand<Results<Ok<CreateCountryResponse>, 
    ProblemDetails, 
    InternalServerError>>
{
  public string CountryName { get; set; } = string.Empty;
  public string LanguageName {  get; set; } = string.Empty;
  public long InitUser { get; set; }
  public required CreateCountryMapper Mapper { get; set; }
}