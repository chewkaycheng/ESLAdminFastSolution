using Dapper;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels;
using ESLAdmin.Features.ChildcareLevels.CreateChildcareLevel;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using System.Threading;

namespace ESLAdmin.Features.Users.RegisterUser;

public class Endpoint : Endpoint<Request, EmptyResponse, Mapper>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly IMessageLogger _messageLogger;
 
  public Endpoint(
    IRepositoryManager repositoryManager,
    IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _messageLogger = messageLogger;
  }

  public override void Configure()
  {
    Post("/api/register");
  }

  public override async Task HandleAsync(Request r, CancellationToken c)
  {
    try
    {
      var response = await _repositoryManager.AuthenticationRepository.RegisterUser(
        r, Map);
      if (!response.IsSuccess)
      {
        foreach(var error in response.Data.Errors)
        {
          AddError(error.Code, error.Description);
        }
        ThrowIfAnyErrors();
      }
      await SendCreatedAtAsync<Endpoint>(
        new EmptyResponse(), 
        cancellation: c);
    }
    catch (Exception ex)
    {
    }
  }
}