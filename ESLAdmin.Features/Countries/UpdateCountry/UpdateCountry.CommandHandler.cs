using Dapper;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.ClassLevels.Endpoints.UpdateClassLevel;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Countries.UpdateCountry;

//------------------------------------------------------------------------------
//
//                           class UpdateCountryCommandHandler
//
//------------------------------------------------------------------------------
public class UpdateCountryCommandHandler :
  CountryCommandHandlerBase<UpdateCountryCommandHandler>,
  ICommandHandler<UpdateCountryCommand,
    Results<Ok<UpdateCountryResponse>,
        ProblemDetails,
        InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                           UpdateCountryCommandHandler
  //
  //------------------------------------------------------------------------------
  public UpdateCountryCommandHandler(
    ICountryRepository repository,
    ILogger<UpdateCountryCommandHandler> logger) : 
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                           ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<UpdateCountryResponse>, 
  ProblemDetails, 
    InternalServerError>> 
      ExecuteAsync(
        UpdateCountryCommand command, 
        CancellationToken ct)
  {
    DynamicParameters parameters = command.Mapper.ToParameters(command);
    var result = await _repository
      .UpdateCountryAsync(
        parameters);
    if (result.IsError)
    {
      var errors = result.Errors;
      return new ProblemDetails(
       ErrorUtils.CreateFailureList(errors),
       StatusCodes.Status500InternalServerError);
    }

    OperationResult operationResult =
      command.Mapper.FromParameters(parameters);
    if (operationResult.DbApiError == 0)
    {
      if (operationResult.Guid == null)
      {
        _logger.LogError("Guid Id for UpdateCountry is null");
        return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            "GuidNull",
            "Guid for for UpdateCountry is null"),
          StatusCodes.Status500InternalServerError);
      }

      UpdateCountryResponse response = new UpdateCountryResponse
      {
        CountryId = command.CountryId,
        Guid = operationResult.Guid
      };
      return TypedResults.Ok(response);
    }

    return AppErrors.ProblemDetailsFactory.OperationResultFailure(
      operationResult.DbApiError,
      operationResult.DupFieldName!);
  }
}
