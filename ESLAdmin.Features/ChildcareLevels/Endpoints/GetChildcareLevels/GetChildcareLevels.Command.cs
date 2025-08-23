using ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevel;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.GetChildcareLevels;

public class GetChildcareLevelsCommand : ICommand<Results<Ok<IEnumerable<GetChildcareLevelResponse>>, ProblemDetails, InternalServerError>>
{
  public required GetChildcareLevelMapper Mapper { get; set; }
}
