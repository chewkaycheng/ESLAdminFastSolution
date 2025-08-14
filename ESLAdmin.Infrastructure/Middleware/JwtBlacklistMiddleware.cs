using ESLAdmin.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace ESLAdmin.Infrastructure.Middleware;

public class JwtBlacklistMiddleware
{
  private readonly RequestDelegate _next;

  public JwtBlacklistMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(
    HttpContext context,
    ITokenBlacklistService tokenBlacklistService)
  {
    if (context.User.Identity?.IsAuthenticated == true)
    {
      var token = context.Request.Headers["Authorization"]
        .ToString().Replace("Bearer ", "");
      if (!string.IsNullOrEmpty(token) && await tokenBlacklistService.IsBlacklistedAsync(token, context.RequestAborted))
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Token has been revoked.");
        return;
      }
    }

    await _next(context);
  }
}
