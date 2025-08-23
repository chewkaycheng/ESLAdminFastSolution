using Dapper;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels;

public class CreateChildcareLevelCommandHandler :
  ChildcareLevelCommandHandlerBase<CreateChildcareLevelCommandHandler>,
  ICommandHandler<CreateChildcareLevelCommand,
  Results<Ok<CreateChildcareLevelResponse>, ProblemDetails, InternalServerError>>
{
  public CreateChildcareLevelCommandHandler(
    IChildcareLevelRepository repository,
    ILogger<CreateChildcareLevelCommandHandler> logger) :
    base(repository, logger)
  {
  }

  public async Task<Results<Ok<CreateChildcareLevelResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      CreateChildcareLevelCommand command,
      CancellationToken cancellationToken)
  {
    DynamicParameters parameters = command.Mapper.ToParameters(command);
    var result = await _repository
      .CreateChildcareLevelAsync(
        parameters);

    if (result.IsError)
    {
      var error = result.Errors.First();
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(
          error.Code,
          error.Description),
        StatusCodes.Status500InternalServerError);
    }

    OperationResult operationResult = command.Mapper.FromParameters(parameters);
    if (operationResult.DbApiError == 100)
    {
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(
          "Duplicate", $"The Childcare level with name {command.ChildcareLevelName} already exists."),
        StatusCodes.Status409Conflict);
    }

    CreateChildcareLevelResponse response = command.Mapper.ToResponse(parameters);
    return TypedResults.Ok(response);
  }
}