using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                           class CreateChildcareLevelCommand
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelCommand : ICommand<Results<Ok<CreateChildcareLevelResponse>, ProblemDetails, InternalServerError>>
{
  public string ChildcareLevelName { get; set; } = string.Empty;
  public int MaxCapacity { get; set; }
  public int DisplayOrder { get; set; }
  public long InitUser { get; set; }
  public required CreateChildcareLevelMapper Mapper { get; set; }
}

