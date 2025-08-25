using ErrorOr;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ClassLevels.Endpoints.DeleteClassLevel;

public class DeleteClassLevelCommand :
  ICommand<Results<Ok<Success>,
    ProblemDetails,
    InternalServerError>>
{
  public long Id { get; set; }
  public required DeleteClassLevelMapper Mapper { get; set; }
}
