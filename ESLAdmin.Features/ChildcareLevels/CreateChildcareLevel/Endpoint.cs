using ESLAdmin.Features;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.CreateChildcareLevel;

public class Endpoint : Endpoint<Request, OperationResult, Mapper>
{
  private readonly IChildcareLevelRepositoryManager _manager;

  public Endpoint(IChildcareLevelRepositoryManager manager)
  {
    _manager = manager;
  }

  public override void Configure()
  {
    Post("route");
  }

  public override async Task HandleAsync(Request r, CancellationToken c)
  {
    var result = await _manager.ChildcareLevel.CreateChildcareLevelAsync(Map, r);
    await SendAsync(result);
  }
}