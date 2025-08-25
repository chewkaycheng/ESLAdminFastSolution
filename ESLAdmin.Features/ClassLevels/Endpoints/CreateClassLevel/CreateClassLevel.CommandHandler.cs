using Dapper;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Repositories.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;


namespace ESLAdmin.Features.ClassLevels.Endpoints.CreateClassLevel;

public class CreateClassLevelCommandHandler :
  ClassLevelCommandHandlerBase<CreateClassLevelCommandHandler>,
  ICommandHandler<CreateClassLevelCommand,
    Results<Ok<CreateClassLevelResponse>,
      ProblemDetails,
      InternalServerError>>
{
  public CreateClassLevelCommandHandler(
    IClassLevelRepository repository, 
    ILogger<CreateClassLevelCommandHandler> logger) : 
    base(repository, logger)
  {
  }

  public async Task<Results<Ok<CreateClassLevelResponse>, 
    ProblemDetails, 
    InternalServerError>> 
      ExecuteAsync(
        CreateClassLevelCommand command, 
        CancellationToken ct)
  {
    DynamicParameters parameters = command
      .Mapper
      .ToParameters(command);
    var result = await _repository
      .CreateClassLevelAsync(
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
        _logger.LogError("Guid Id for CreateClassLevel is null");
        return new ProblemDetails(
          ErrorUtils.CreateFailureList(
            "GuidNull",
            "Guid for for CreateClassLevel is null"),
          StatusCodes.Status500InternalServerError);
      }

      CreateClassLevelResponse response = command.Mapper.ToResponse(parameters);
      return TypedResults.Ok(response);
    }

    return AppErrors.ProblemDetailsFactory.OperationResultFailure(
      operationResult.DbApiError,
      operationResult.DupFieldName);
  }
}