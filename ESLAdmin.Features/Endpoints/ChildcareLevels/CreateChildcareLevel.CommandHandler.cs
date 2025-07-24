using Dapper;
using ESLAdmin.Features.Exceptions;
using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

public class CreateChildcareLevelCommandHandler : ICommandHandler<
    CreateChildcareLevelCommand,
    Results<NoContent, ProblemDetails, InternalServerError>>
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

  public async Task<Results<NoContent, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      CreateChildcareLevelCommand command,
      CancellationToken cancellationToken)
  {
    try
    {
      DynamicParameters parameters = command.Mapper.ToEntity(command);
      await _repositoryManager.ChildcareLevelRepository.CreateChildcareLevelAsync(
        parameters);

      OperationResult operationResult = command.Mapper.FromEntity(parameters);
      if (operationResult.DbApiError != 100)
      {
        var validationFailures = new List<ValidationFailure>();
        validationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "Duplicate",
          ErrorMessage = $"The Childcare level with name {command.ChildcareLevelName} already exists."
        });
        return new ProblemDetails(validationFailures, StatusCodes.Status409Conflict);
      }
      return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
      _messageLogger.LogControllerException(
        nameof(ExecuteAsync),
        ex);

      return TypedResults.InternalServerError();
    }
  }
}
