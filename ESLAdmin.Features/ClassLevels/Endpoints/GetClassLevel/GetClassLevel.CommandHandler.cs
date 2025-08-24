using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.ClassLevels.Infrastructure.Persistence.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevel;

//------------------------------------------------------------------------------
//
//                        class GetClassLevelCommandHandler
//
//------------------------------------------------------------------------------
public class GetClassLevelCommandHandler :
  ClassLevelCommandHandlerBase<GetClassLevelCommandHandler>,
  ICommandHandler<GetClassLevelCommand,
    Results<Ok<GetClassLevelResponse>,
      ProblemDetails,
      InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                        GetClassLevelCommandHandler
  //
  //------------------------------------------------------------------------------
  public GetClassLevelCommandHandler(
    IClassLevelRepository repository, 
    ILogger<GetClassLevelCommandHandler> logger) : 
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<GetClassLevelResponse>, 
    ProblemDetails, 
    InternalServerError>> 
    ExecuteAsync(
      GetClassLevelCommand command, 
      CancellationToken ct)
  {
    var parameters = command.Mapper.ToParameters(command.Id);
    var classLevelResult = await _repository
      .GetClassLevelAsync(parameters);
    if (classLevelResult.IsError)
    {
      var errors = classLevelResult.Errors;
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(errors),
        StatusCodes.Status500InternalServerError);
    }

    var classLevel = classLevelResult.Value;
    if (classLevel == null)
    {
      return new ProblemDetails(ErrorUtils.CreateFailureList(
        "NotFound",
        $"The childcare level with id: {command.Id} is not found."
      ), StatusCodes.Status404NotFound);
    }

    var classLevelResponse = command.Mapper.FromEntity(classLevel);
    return TypedResults.Ok(classLevelResponse);
  }
}
