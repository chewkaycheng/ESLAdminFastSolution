using ErrorOr;
using ESLAdmin.Features.ClassLevels.Endpoints.DeleteClassLevel;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Countries.DeleteCountry;

//------------------------------------------------------------------------------
//
//                    class DeleteCountryCommand
//
//------------------------------------------------------------------------------
public class DeleteCountryCommand :
  ICommand<Results<Ok<Success>,
    ProblemDetails,
    InternalServerError>>
{
  public long Id { get; set; }
  public required DeleteCountryMapper Mapper { get; set; }
}
