using ESLAdmin.Infrastructure.Services;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Middleware;

public class JwtBlacklistMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<JwtBlacklistMiddleware> _logger;

  public JwtBlacklistMiddleware(
    RequestDelegate next,
    ILogger<JwtBlacklistMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(
    HttpContext context,
    ITokenBlacklistService tokenBlacklistService)
  {
    if (context.User.Identity?.IsAuthenticated == true)
    {
      var token = context.Request.Headers["Authorization"]
        .ToString().Replace("Bearer ", "");

      if (string.IsNullOrEmpty(token))
      {
        _logger.LogWarning("No token provided in Authorization header for authenticated request.");
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Missing token.");
        return;
      }

      var result = await tokenBlacklistService.IsBlacklistedAsync(token, context.RequestAborted);
      if (result.IsError)
      {
        var error = result.Errors.First();
        _logger.LogError("Failed to check blacklist for token: {Error}", result.FirstError.Description);
        if (error.Code == "Database.OperationCanceled")
        {
          context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
          await context.Response.WriteAsync("The operation was canceled.");
          return;
        }

        // Default to 401 for safety, as we can't confirm token validity
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Token validation failed.");
        return;
      }

      var isBlacklisted = result.Value;
      if (!string.IsNullOrEmpty(token) && isBlacklisted)
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Token has been revoked.");
        return;
      }
    }

    await _next(context);
  }
}
