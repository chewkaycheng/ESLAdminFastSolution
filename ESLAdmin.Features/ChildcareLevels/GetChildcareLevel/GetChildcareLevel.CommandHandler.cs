using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.GetChildcareLevel;

//------------------------------------------------------------------------------
//
//                        class GetChildcareLevelHandler
//
//------------------------------------------------------------------------------
public class GetChildcareLevelHandler : ICommandHandler<
  GetChildcareLevelCommand,
  Results<Ok<GetChildcareLevelResponse>, ProblemDetails, InternalServerError>>
{
  private readonly IChildcareLevelRepository _repository;
  private readonly IMessageLogger _messageLogger;

  public GetChildcareLevelHandler(
    IChildcareLevelRepository repository,
    IMessageLogger messageLogger)
  {
    _repository = repository;
    _messageLogger = messageLogger;
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