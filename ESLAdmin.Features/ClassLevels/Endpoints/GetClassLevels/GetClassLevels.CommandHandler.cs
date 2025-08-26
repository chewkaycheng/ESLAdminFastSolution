using ESLAdmin.Common.CustomErrors;
using ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevel;
using ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevel;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Repositories.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevels;

//------------------------------------------------------------------------------
//
//                        class GetClassLevelsCommandHandler
//
//------------------------------------------------------------------------------
public class GetClassLevelsCommandHandler :
  ClassLevelCommandHandlerBase<GetClassLevelsCommandHandler>,
  ICommandHandler<
    GetClassLevelsCommand,
    Results<Ok<IEnumerable<GetClassLevelResponse>>,
      ProblemDetails,
      InternalServerError>>
{
  //------------------------------------------------------------------------------
  //
  //                        GetClassLevelsCommandHandler
  //
  //------------------------------------------------------------------------------
  public GetClassLevelsCommandHandler(
    IClassLevelRepository repository, 
    ILogger<GetClassLevelsCommandHandler> logger) : 
    base(repository, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<IEnumerable<GetClassLevelResponse>>, 
    ProblemDetails, 
    InternalServerError>> 
    ExecuteAsync(
      GetClassLevelsCommand command, 
      CancellationToken ct)
  {
    var classLevelsResult =
      await _repository
        .GetClassLevelsAsync();

    if (classLevelsResult.IsError)
    {
      var errors = classLevelsResult.Errors;
      return new ProblemDetails(
        ErrorUtils.CreateFailureList(errors),
        StatusCodes.Status500InternalServerError);
    }

    var classLevels = classLevelsResult.Value;
    IEnumerable<GetClassLevelResponse> classLevelsResponse =
      classLevels.Select(classLevel => command.Mapper.FromEntity(
        classLevel)).ToList();

    return TypedResults.Ok(classLevelsResponse);
  }
}