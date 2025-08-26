using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Countries.GetCountry;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Countries.GetCountries;

//------------------------------------------------------------------------------
//
//                        class GetCountrysCommandHandler
//
//------------------------------------------------------------------------------
public class GetCountriesCommandHandler :
  CountryCommandHandlerBase<GetCountriesCommandHandler>,
  ICommandHandler<
    GetCountriesCommand,
    Results<Ok<IEnumerable<GetCountryResponse>>,
      ProblemDetails,
      InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                        GetCountrysCommandHandler
  //
  //------------------------------------------------------------------------------
  public GetCountriesCommandHandler(
    ICountryRepository repository,
    ILogger<GetCountriesCommandHandler> logger) :
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<IEnumerable<GetCountryResponse>>,
      ProblemDetails,
      InternalServerError>>
    ExecuteAsync(
      GetCountriesCommand command,
      CancellationToken ct)
  {
    var CountriesResult =
      await _repository
        .GetCountriesAsync();

    if (CountriesResult.IsError)
    {
      var errors = CountriesResult.Errors;
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(errors),
        StatusCodes.Status500InternalServerError);
    }

    var Countries = CountriesResult.Value;
    IEnumerable<GetCountryResponse> CountriesResponse =
      Countries.Select(Country => command.Mapper.FromEntity(
        Country)).ToList();

    return TypedResults.Ok(CountriesResponse);
  }
}
