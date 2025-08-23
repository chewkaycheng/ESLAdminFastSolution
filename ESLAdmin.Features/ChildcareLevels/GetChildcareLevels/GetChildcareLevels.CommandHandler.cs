using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Infrastructure.Persistence.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels.GetChildcareLevel;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelsCommandHandler
//
//------------------------------------------------------------------------------
public class GetChildcareLevelsCommandHandler : ICommandHandler<
  GetChildcareLevelsCommand,
  Results<Ok<IEnumerable<GetChildcareLevelResponse>>, ProblemDetails, InternalServerError>>
{
  private readonly IChildcareLevelRepository _repository;
  private readonly ILogger<GetChildcareLevelsCommandHandler> _logger;

  public GetChildcareLevelsCommandHandler(
    IChildcareLevelRepository repository,
    ILogger<GetChildcareLevelsCommandHandler> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  public async Task<Results<Ok<IEnumerable<GetChildcareLevelResponse>>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      GetChildcareLevelsCommand command,
      CancellationToken cancellationToken)
  {
    var childcareLevelsResult =
      await _repository
        .GetChildcareLevelsAsync();

    if (childcareLevelsResult.IsError)
    {
      var errors = childcareLevelsResult.Errors;
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(errors),
        StatusCodes.Status500InternalServerError);
    }

    var childcareLevels = childcareLevelsResult.Value;
    IEnumerable<GetChildcareLevelResponse> childcareLevelsResponse =
      childcareLevels.Select(childcareLevel => command.Mapper.FromEntity(
        childcareLevel)).ToList();

    return TypedResults.Ok(childcareLevelsResponse);
  }
}