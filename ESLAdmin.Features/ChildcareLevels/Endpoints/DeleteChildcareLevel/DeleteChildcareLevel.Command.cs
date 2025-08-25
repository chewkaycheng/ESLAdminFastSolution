using ErrorOr;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.DeleteChildcareLevel;

//------------------------------------------------------------------------------
//
//                        DeleteChildcareLevelCommand
//
//------------------------------------------------------------------------------
public class DeleteChildcareLevelCommand :
  ICommand<Results<Ok<Success>, ProblemDetails, InternalServerError>>
{
  public long Id { get; set; }
  public required DeleteChildcareLevelMapper Mapper { get; set; }
}
