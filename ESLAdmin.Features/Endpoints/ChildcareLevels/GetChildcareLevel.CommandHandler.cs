using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelHandler
//
//------------------------------------------------------------------------------
public class GetChildcareLevelHandler : ICommandHandler<
  GetChildcareLevelCommand,
  Results<Ok<GetChildcareLevelResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly IMessageLogger _messageLogger;

  public GetChildcareLevelHandler(
    IRepositoryManager repositoryManager,
    IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _messageLogger = messageLogger;
  }

  public async Task<Results<Ok<GetChildcareLevelResponse>, ProblemDetails, InternalServerError>>
  ExecuteAsync(
    GetChildcareLevelCommand command,
    CancellationToken cancellationToken)
  {
    try
    {
      var parameters = command.Mapper.ToParameters(command.Id);
      var childcareLevelResult = await _repositoryManager
                                        .ChildcareLevelRepository
                                        .GetChildcareLevelAsync(parameters);
      if (childcareLevelResult.IsError)
      {
        return TypedResults.InternalServerError();
      }

      var childcareLevel = childcareLevelResult.Value;
      if (childcareLevel == null)
      {
        return new ProblemDetails(ErrorUtils.CreateFailureList(
          "NotFound",
          $"The childcare level with id: {command.Id} is not found."
          ), StatusCodes.Status404NotFound);
      }

      var childcareLevelResponse = command.Mapper.FromEntity(childcareLevel);
      return TypedResults.Ok(childcareLevelResponse);
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(ExecuteAsync), ex);
      return TypedResults.InternalServerError();
    }
  }
}
