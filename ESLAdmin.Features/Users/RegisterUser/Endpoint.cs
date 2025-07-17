using Dapper;
using ESLAdmin.Features.ChildcareLevels;
using ESLAdmin.Features.ChildcareLevels.CreateChildcareLevel;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.Users.RegisterUser;

public class Endpoint : Endpoint<Request, APIResponse<OperationResult>, Mapper>
{
  public override void Configure()
  {
    Post("/api/users");
  }

  public override async Task HandleAsync(Request r, CancellationToken c)
  {
    try
    {
    }
    catch (Exception ex)
    {
    }
  }
}