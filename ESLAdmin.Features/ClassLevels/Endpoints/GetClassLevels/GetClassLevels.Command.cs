using ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevel;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevels;

public class GetClassLevelsCommand : 
  ICommand<Results<Ok<IEnumerable<GetClassLevelResponse>>,
    ProblemDetails,
    InternalServerError>>
{
  public long Id { get; set; }
  public required GetClassLevelMapper Mapper { get; set; }
}