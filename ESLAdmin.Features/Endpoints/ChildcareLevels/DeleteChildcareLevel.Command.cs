using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                        DeleteChildcareLevelCommand
//
//------------------------------------------------------------------------------
public class DeleteChildcareLevelCommand :
  ICommand<Results<NoContent, ProblemDetails, InternalServerError>>
{
  public long Id { get; set; }
  public required DeleteChildcareLevelMapper Mapper { get; set; }
}
