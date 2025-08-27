using Dapper;
using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.ClassLevels.Endpoints;
using ESLAdmin.Features.ClassLevels.Endpoints.DeleteClassLevel;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Repositories;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Countries.DeleteCountry;

//------------------------------------------------------------------------------
//
//                    class DeleteCountryCommandHandler
//
//------------------------------------------------------------------------------
public class DeleteCountryCommandHandler :
  CountryCommandHandlerBase<DeleteCountryCommandHandler>,
  ICommandHandler<DeleteCountryCommand,
    Results<Ok<Success>,
      ProblemDetails,
      InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                    DeleteCountryCommandHandler
  //
  //------------------------------------------------------------------------------
  public DeleteCountryCommandHandler(
  ICountryRepository repository,
    ILogger<DeleteCountryCommandHandler> logger) :
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                    ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<Success>,
    ProblemDetails,
    InternalServerError>>
      ExecuteAsync(
        DeleteCountryCommand command,
        CancellationToken ct)
  {
    _logger.LogFunctionEntry($"\n=>CountryId: '{command.Id}'");
    DynamicParameters parameters = command.Mapper.ToParameters(command.Id);
    var result = await _repository.
      DeleteCountryAsync(
        parameters);

    if (result.IsError)
    {
      var errors = result.Errors;
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(errors),
        StatusCodes.Status500InternalServerError);
    }

    OperationResult operationResult = command
      .Mapper
      .FromParameters(parameters);
    if (operationResult.DbApiError == 0)
    {
      _logger.LogFunctionExit();
      return TypedResults.Ok(new Success());
    }

    _logger.LogFunctionExit();
    return new ProblemDetails(ErrorUtils.CreateFailureList(
        "Database.ConcurrencyConflict",
        $"Cannot delete Country. It is being used by {operationResult.ReferenceTable}."),
      StatusCodes.Status409Conflict);
  }
}
