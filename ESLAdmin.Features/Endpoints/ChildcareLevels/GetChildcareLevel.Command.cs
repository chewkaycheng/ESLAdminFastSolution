using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels
{
  public class GetChildcareLevelCommand : ICommand<Results<Ok<GetChildcareLevelResponse>,
    ProblemDetails,
    InternalServerError>>
  {
    public long Id { get; set; }
    public required GetChildcareLevelMapper Mapper { get; set; }
  }
}
