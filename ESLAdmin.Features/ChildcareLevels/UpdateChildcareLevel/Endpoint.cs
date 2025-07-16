using ESLAdmin.Features;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.UpdateChildcareLevel;

public class Endpoint : Endpoint<Request, OperationResult, Mapper>
{
  public override void Configure()
  {
    Post("route");
  }

  public override async Task HandleAsync(Request r, CancellationToken c)
  {
    await SendAsync(new Response());
  }
}