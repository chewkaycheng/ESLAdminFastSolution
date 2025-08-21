using ErrorOr;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Numerics;

namespace ESLAdmin.Common.CustomErrors;

public static partial class AppErrors
{
  public class CustomProblemDetails : ProblemDetails
  {
    public Dictionary<string, object>? Metadata { get; set; }
  }

  public static class ProblemDetailsFactory
  {
    public static ProblemDetails CreateProblemDetails(
      string code,
      string description,
      int statusCode,
      Dictionary<string, object>? metadata = null)
    {
      if (metadata != null && metadata.Any())
      {
        return new ProblemDetails(
          ErrorUtils.CreateFailureList(metadata));
      }

      return new ProblemDetails(
        ErrorUtils.CreateFailureList(
          code,
          description),
        statusCode);
    }

    public static ProblemDetails AuthenticationFailed()
    {
      return CreateProblemDetails(
        "AuthenticationFailed",
        "Invalid request: Authentication failed.",
        StatusCodes.Status401Unauthorized);
    }

    public static ProblemDetails LockedOut()
    {
      return CreateProblemDetails(
        "LockedOut",
        "Your account has been locked. Please contact your administrator to unlock your account.",
        StatusCodes.Status401Unauthorized);
    }

    public static ProblemDetails RequiresTwoFactor()
    {
      return CreateProblemDetails(
        "RequiresTwoFactor",
        "Two factor authentication required.",
        StatusCodes.Status401Unauthorized);
    }

    public static ProblemDetails LoginFailed()
    {
      return CreateProblemDetails(
        "LoginFailed",
        "Username or password is invalid.",
        StatusCodes.Status401Unauthorized);
    }

    public static ProblemDetails TokenError()
    {
      return CreateProblemDetails(
        "TokenError",
        "Invalid or missing token.",
        StatusCodes.Status401Unauthorized);
    }

    public static ProblemDetails InvalidLogoutRequest()
    {
      return CreateProblemDetails(
        "TokenError",
        "Invalid logout request.",
        StatusCodes.Status400BadRequest);
    }

    public static ProblemDetails RequestTimeout()
    {
      return CreateProblemDetails(
        "RequestTimeout",
        "Request has timed out.",
        StatusCodes.Status408RequestTimeout);
    }

    public static ProblemDetails ConcurrencyFailure()
    {
      return CreateProblemDetails(
        "Database.ConcurrencyFailure",
        "There is a concurrency failure in the operation.",
        StatusCodes.Status409Conflict);
    }
  }
}
