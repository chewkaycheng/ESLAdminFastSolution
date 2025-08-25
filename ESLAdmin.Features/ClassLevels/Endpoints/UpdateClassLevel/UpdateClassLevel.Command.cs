using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ClassLevels.Endpoints.UpdateClassLevel;

public class UpdateClassLevelCommand :
  ICommand<Results<Ok<UpdateClassLevelResponse>,
    ProblemDetails,
    InternalServerError>>
{
  public long ClassLevelId { get; set; }
  public string ClassLevelName { get; set; } = string.Empty;
  public int DisplayOrder { get; set; } = 0;
  public int DisplayColor { get; set; } = 0;
  public long UserCode { get; set; }
  public string Guid { get; set; } = string.Empty;
  public required UpdateClassLevelMapper Mapper { get; set; }
}