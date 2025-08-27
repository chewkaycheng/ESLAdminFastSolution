using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Countries.UpdateCountry;

//------------------------------------------------------------------------------
//
//                           class UpdateCountryCommand
//
//------------------------------------------------------------------------------
public class UpdateCountryCommand : 
  ICommand<Results<Ok<UpdateCountryResponse>,
    ProblemDetails,
    InternalServerError>>
{
  public long CountryId { get; set; }
  public string CountryName { get; set; } = string.Empty;
  public string LanguageName { get; set; } = string.Empty;
  public long UserCode { get; set; }
  public string Guid { get; set; } = string.Empty;
  public required UpdateCountryMapper Mapper { get; set; }
}
