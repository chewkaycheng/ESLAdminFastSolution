using ESLAdmin.Domain.Entities;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.GetChildcareLevels;

public class Endpoint : Endpoint<EmptyRequest, IEnumerable<Response>, Mapper>
{
  public override void Configure()
  {
    Get("/api/childcarelevels");
    AllowAnonymous();
  }

  public override async Task HandleAsync(EmptyRequest req, CancellationToken c)
  {
    var models = new List<ChildcareLevel>();
    var ch = new ChildcareLevel();
    ch.Id = 1;
    ch.ChildcareLevelName = "Infant";
    ch.MaxCapacity = 22;
    ch.DisplayOrder = 1;
    ch.PlacesAssigned = 12;
    ch.InitUser = 0;
    ch.InitDate = DateTime.Now;
    ch.UserCode = 1;
    ch.UserStamp = DateTime.Now;
    ch.Guid = Guid.NewGuid().ToString();
    models.Add(ch);

    ch = new ChildcareLevel();
    ch.Id = 2;
    ch.ChildcareLevelName = "Toddler";
    ch.MaxCapacity = 24;
    ch.DisplayOrder = 2;
    ch.PlacesAssigned = 12;
    ch.InitUser = 0;
    ch.InitDate = DateTime.Now;
    ch.UserCode = 1;
    ch.UserStamp = DateTime.Now;
    ch.Guid = Guid.NewGuid().ToString();
    models.Add(ch);

    var response = models.Select(childcareLevel => Map.FromEntity(
      childcareLevel)).ToList();

    await SendAsync(response);
  }
}