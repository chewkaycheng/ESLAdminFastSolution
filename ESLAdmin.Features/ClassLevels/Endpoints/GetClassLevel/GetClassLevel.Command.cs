using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevel;

//------------------------------------------------------------------------------
//
//                        class GetClassLevelCommand
//
//------------------------------------------------------------------------------
public class GetClassLevelCommand : 
  ICommand<Results<Ok<GetClassLevelResponse>,
    ProblemDetails,
    InternalServerError>>
{
  public long Id { get; set; }
  public required GetClassLevelMapper Mapper { get; set; }
}