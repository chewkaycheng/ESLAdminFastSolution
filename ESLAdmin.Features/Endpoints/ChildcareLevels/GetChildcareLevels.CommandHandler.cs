using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

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
  private readonly ILogger<GetChildcareLevelsCommandHandler> _logger;

  public GetChildcareLevelsCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<GetChildcareLevelsCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  public async Task<Results<Ok<IEnumerable<GetChildcareLevelResponse>>, ProblemDetails, InternalServerError>>
  ExecuteAsync(
    GetChildcareLevelsCommand command,
    CancellationToken cancellationToken)
  {
    try
    {
      var childcareLevelsResult = await _repositoryManager
                                    .ChildcareLevelRepository
                                    .GetChildcareLevelsAsync();
      if (childcareLevelsResult.IsError)
      {
        return TypedResults.InternalServerError();
      }

      var childcareLevels = childcareLevelsResult.Value;
      IEnumerable<GetChildcareLevelResponse> childcareLevelsResponse =
        childcareLevels.Select(
          childcareLevel => command.Mapper.FromEntity(
            childcareLevel)).ToList();

      return TypedResults.Ok(childcareLevelsResponse);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);

      return TypedResults.InternalServerError();
    }
  }
}
