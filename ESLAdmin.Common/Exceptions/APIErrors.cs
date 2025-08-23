using FluentValidation.Results;

namespace ESLAdmin.Common.Exceptions;

//------------------------------------------------------------------------------
//
//                       class APIErrors
//
//------------------------------------------------------------------------------

public class ApiErrors
{
  public required List<ValidationFailure> Errors { get; set; }
}
