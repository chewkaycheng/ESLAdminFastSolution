using Dapper;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.ClassLevels.Endpoints.CreateClassLevel;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Countries.CreateCountry;

public class CreateCountryCommandHandler :
  CountryCommandHandlerBase<CreateCountryCommandHandler>,
  ICommandHandler<CreateCountryCommand,
    Results<Ok<CreateCountryResponse>,
      ProblemDetails,
      InternalServerError>>
{
  public CreateCountryCommandHandler(
    ICountryRepository repository, 
    ILogger<CreateCountryCommandHandler> logger) : 
    base(repository, logger)
  {
  }

  public async Task<Results<Ok<CreateCountryResponse>, 
    ProblemDetails, 
    InternalServerError>> 
      ExecuteAsync(
        CreateCountryCommand command, 
        CancellationToken ct)
  {
    DynamicParameters parameters = command
      .Mapper
      .ToParameters(command);
    var result = await _repository
      .CreateCountryAsync(
      parameters);

    if (result.IsError)
    {
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(
          result.Errors),
        StatusCodes.Status500InternalServerError
      );
    }

    OperationResult operationResult = command.Mapper.FromParameters(parameters);
    if (operationResult.DbApiError == 0)
    {
      if (operationResult.Guid == null)
      {
        _logger.LogError("Guid Id for CreateCountry is null");
        return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            "GuidNull",
            "Guid for for CreateCountry is null"),
          StatusCodes.Status500InternalServerError);
      }

      CreateCountryResponse response = command
        .Mapper
        .ToResponse(parameters);
      return TypedResults.Ok(response);
    }

    return AppErrors
      .ProblemDetailsFactory
      .OperationResultFailure(
        operationResult.DbApiError,
        operationResult.DupFieldName!);
  }
}
