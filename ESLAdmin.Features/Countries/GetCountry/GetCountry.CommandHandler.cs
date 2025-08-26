using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Countries.GetCountry;

//------------------------------------------------------------------------------
//
//                        class GetCountryCommandHandler
//
//------------------------------------------------------------------------------
public class GetCountryCommandHandler :
  CountryCommandHandlerBase<GetCountryCommandHandler>,
  ICommandHandler<GetCountryCommand,
    Results<Ok<GetCountryResponse>,
      ProblemDetails,
      InternalServerError>>
{
  public GetCountryCommandHandler(
    ICountryRepository repository, 
    ILogger<GetCountryCommandHandler> logger) : 
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<GetCountryResponse>, 
    ProblemDetails, 
    InternalServerError>> 
    ExecuteAsync(
      GetCountryCommand command, 
      CancellationToken ct)
  {
    var parameters = command.Mapper.ToParameters(command.Id);
    var countryResult = await _repository
      .GetCountryAsync(parameters);
    if (countryResult.IsError)
    {
      var errors = countryResult.Errors;
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(errors),
        StatusCodes.Status500InternalServerError);
    }

    var country = countryResult.Value;
    if (country == null)
    {
      return new ProblemDetails(ErrorUtils.CreateFailureList(
          "NotFound",
          $"The country with id: '{command.Id}' is not found."),
        StatusCodes.Status404NotFound);
    }

    var countryResponse = command.Mapper.FromEntity(country);
    return TypedResults.Ok(countryResponse);
  }
}