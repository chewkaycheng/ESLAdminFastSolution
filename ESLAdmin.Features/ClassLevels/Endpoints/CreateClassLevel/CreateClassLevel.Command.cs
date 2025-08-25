using ESLAdmin.Features.ChildcareLevels.Endpoints.CreateChildcareLevel;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ClassLevels.Endpoints.CreateClassLevel;

//------------------------------------------------------------------------------
//
//                           class CreateClassLevelCommand
//
//------------------------------------------------------------------------------
public class CreateClassLevelCommand : 
  ICommand<Results<Ok<CreateClassLevelResponse>, ProblemDetails, InternalServerError>>
{
  public string ClassLevelName { get; set; } = string.Empty;
  public int DisplayOrder { get; set; }
  public int DisplayColor { get; set; }
  public long InitUser { get; set; }
  public required CreateClassLevelMapper Mapper { get; set; }
}
