using ErrorOr;
using ESLAdmin.Common.CustomErrors;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Infrastructure.Persistence.Identity;

//------------------------------------------------------------------------------
//
//                      static class IdentityResultMapper
//
//------------------------------------------------------------------------------
public static class IdentityResultMapper
{
  public static Results<Ok<T>, ProblemDetails, InternalServerError> MapToResult<T>(Error error)
  {
    var statusCode = error.Code switch
    {
      "Identity.UserNotFound" => StatusCodes.Status404NotFound,
      "Identity.RoleNotFound" => StatusCodes.Status400BadRequest,
      "Identity.UserAlreadyInRole" => StatusCodes.Status400BadRequest,
      "Identity.InvalidRoleName" => StatusCodes.Status400BadRequest,
      "Identity.ConcurrencyError" => StatusCodes.Status409Conflict,
      _ => StatusCodes.Status500InternalServerError
    };

    if (statusCode == StatusCodes.Status500InternalServerError)
    {
      return TypedResults.InternalServerError();
    }

    return new ProblemDetails(
        ErrorUtils.CreateFailureList(error.Code, error.Description),
        statusCode);
  }
}
