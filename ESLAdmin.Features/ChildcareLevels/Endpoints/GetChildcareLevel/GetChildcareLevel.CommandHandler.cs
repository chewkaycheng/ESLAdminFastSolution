using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevel;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelCommandHandler
//
//------------------------------------------------------------------------------
public class GetChildcareLevelCommandHandler : 
  ChildcareLevelCommandHandlerBase<GetChildcareLevelCommandHandler>, 
  ICommandHandler<GetChildcareLevelCommand,
    Results<Ok<GetChildcareLevelResponse>, ProblemDetails, InternalServerError>>
{
  public GetChildcareLevelCommandHandler(
    IChildcareLevelRepository repository,
    ILogger<GetChildcareLevelCommandHandler> logger) :
    base(repository, logger)
  {
  }

  public async Task<Results<Ok<GetChildcareLevelResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      GetChildcareLevelCommand command,
      CancellationToken cancellationToken)
  {
    var parameters = command.Mapper.ToParameters(command.Id);
    var childcareLevelResult = await _repository
      .GetChildcareLevelAsync(parameters);
    if (childcareLevelResult.IsError)
    {
      var errors = childcareLevelResult.Errors;
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(errors),
        StatusCodes.Status500InternalServerError);
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
}