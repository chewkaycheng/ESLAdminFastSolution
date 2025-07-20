using FluentValidation.Results;

namespace ESLAdmin.Features.Exceptions;

//------------------------------------------------------------------------------
//
//                       class APIErrors
//
//------------------------------------------------------------------------------

public class APIErrors
{
  public List<ValidationFailure> Errors { get; set; }
}
