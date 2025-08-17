using Dapper;
using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

public class CreateChildcareLevelCommandHandler : ICommandHandler<
    CreateChildcareLevelCommand,
    Results<Ok<CreateChildcareLevelResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<CreateChildcareLevelCommandHandler> _logger;
  private readonly IMessageLogger _messageLogger;

  public CreateChildcareLevelCommandHandler(
      IRepositoryManager repositoryManager,
      ILogger<CreateChildcareLevelCommandHandler> logger,
      IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
    _messageLogger = messageLogger;
  }

  public async Task<Results<Ok<CreateChildcareLevelResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      CreateChildcareLevelCommand command,
      CancellationToken cancellationToken)
  {
    try
    {
      DynamicParameters parameters = command.Mapper.ToParameters(command);
      var result = await _repositoryManager.ChildcareLevelRepository.CreateChildcareLevelAsync(
        parameters);

      if (result.IsError)
      {
        var error = result.Errors.First();
        if (error.Code != "Database.StoredProcedureError")
        {
          return TypedResults.InternalServerError();
        }
        OperationResult operationResult = command.Mapper.FromParameters(parameters);
        if (operationResult.DbApiError == 100)
        {
          return new ProblemDetails(
            ErrorUtils.CreateFailureList(
              "Duplicate", $"The Childcare level with name {command.ChildcareLevelName} already exists."), StatusCodes.Status409Conflict);
        }
      }

      CreateChildcareLevelResponse response = command.Mapper.ToResponse(parameters);
      return TypedResults.Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}
