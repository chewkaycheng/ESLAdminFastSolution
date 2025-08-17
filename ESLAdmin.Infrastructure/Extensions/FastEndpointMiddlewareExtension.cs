using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ESLAdmin.Infrastructure.Extensions;

public static class FastEndpointMiddlewareExtension
{
  public static void UseFastEndpointsMiddleware(this IApplicationBuilder app)
  {
    app.UseFastEndpoints(c =>
    {
      c.Errors.UseProblemDetails();
      c.Endpoints.Configurator =
        ep =>
        {
          if (ep.AnonymousVerbs is null)
            ep.Description(b => b.Produces<ProblemDetails>(401));
        };
    });
  }
}
