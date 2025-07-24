using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelsCommandHandler
//
//------------------------------------------------------------------------------
public class GetChildcareLevelsCommandHandler : ICommandHandler<
  GetChildcareLevelsCommand,
  Results<Ok<IEnumerable<GetChildcareLevelResponse>>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly IMessageLogger _messageLogger;

  public GetChildcareLevelsCommandHandler(
    IRepositoryManager repositoryManager,
    IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _messageLogger = messageLogger;
  }

  public async Task<Results<Ok<IEnumerable<GetChildcareLevelResponse>>, ProblemDetails, InternalServerError>>
  ExecuteAsync(
    GetChildcareLevelsCommand command,
    CancellationToken cancellationToken)
  {
    try
    {
      var childcareLevels = await _repositoryManager
                                    .ChildcareLevelRepository
                                    .GetChildcareLevelsAsync();

      IEnumerable<GetChildcareLevelResponse> childcareLevelsResponse =
        childcareLevels.Select(
          childcareLevel => command.Mapper.FromEntity(
            childcareLevel)).ToList();

      return TypedResults.Ok(childcareLevelsResponse);
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(nameof(ExecuteAsync), ex);

      return TypedResults.InternalServerError();
    }
  }
}
