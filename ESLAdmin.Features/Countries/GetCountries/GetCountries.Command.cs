using ESLAdmin.Features.Countries.GetCountry;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Countries.GetCountries;

//------------------------------------------------------------------------------
//
//                        class GetCountriesCommand
//
//------------------------------------------------------------------------------
public class GetCountriesCommand :
  ICommand<Results<Ok<IEnumerable<GetCountryResponse>>,
    ProblemDetails,
    InternalServerError>>
{
  public long Id { get; set; }
  public required GetCountryMapper Mapper { get; set; }
}