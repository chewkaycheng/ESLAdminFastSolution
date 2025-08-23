using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevel;
using ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevels;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelsCommandHandler
//
//------------------------------------------------------------------------------
public class GetChildcareLevelsCommandHandler :
  ChildcareLevelCommandHandlerBase<GetChildcareLevelsCommandHandler>,
  ICommandHandler<
  GetChildcareLevelsCommand,
  Results<Ok<IEnumerable<GetChildcareLevelResponse>>, ProblemDetails, InternalServerError>>
{
  public GetChildcareLevelsCommandHandler(
    IChildcareLevelRepository repository,
    ILogger<GetChildcareLevelsCommandHandler> logger) :
    base(repository, logger)
  {
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