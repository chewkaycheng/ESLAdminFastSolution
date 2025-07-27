using Dapper;
using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
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
      await _repositoryManager.ChildcareLevelRepository.CreateChildcareLevelAsync(
        parameters);

      OperationResult operationResult = command.Mapper.FromParameters(parameters);

      if (operationResult.DbApiError == 100)
      {
        var validationFailures = new List<ValidationFailure>();
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "Duplicate",
          ErrorMessage = $"The Childcare level with name {command.ChildcareLevelName} already exists."
        });
        return new ProblemDetails(validationFailures, StatusCodes.Status409Conflict);
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
