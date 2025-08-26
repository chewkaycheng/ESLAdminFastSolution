using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Countries.GetCountry;

public class GetCountryCommand :
  ICommand<Results<Ok<GetCountryResponse>,
    ProblemDetails,
    InternalServerError>>
{
  public long Id { get; set; }
  public required GetCountryMapper Mapper { get; set; }
}