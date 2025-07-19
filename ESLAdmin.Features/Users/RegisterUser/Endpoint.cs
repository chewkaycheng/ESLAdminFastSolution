using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace ESLAdmin.Features.Users.RegisterUser;

public class RegisterUserEndpoint : Endpoint<Request, Object, Mapper>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly IMessageLogger _messageLogger;
 
  public RegisterUserEndpoint(
    IRepositoryManager repositoryManager,
    IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _messageLogger = messageLogger;
  }

  public override void Configure()
  {
    Post("/api/register");
    AllowAnonymous();
  }

  public override async Task HandleAsync(Request r, CancellationToken c)
  {
    try
    {
      var response = await _repositoryManager.AuthenticationRepository.RegisterUser(
        r, Map);
       
      if (!response.IsSuccess)
      {
        foreach(var error in response.Data.IdentityResult.Errors)
        {
          AddError(error.Code, error.Description);
        }
        ThrowIfAnyErrors();
      }

      HttpContext.Response.Headers.Append(
        "location", $"/api/GetUser/{response.Data.User.Id}");

      await SendAsync(new EmptyResponse(), 201, cancellation: c);
      //await SendCreatedAtAsync<ReigsterUserEndpoint>(
      //  routeValues: new { id = response.Data.User.Id },
      //  new EmptyResponse(), 
      //  cancellation: c);
    }
    catch (ValidationFailureException ex)
    {
      _messageLogger.LogControllerException(
        nameof(HandleAsync),
        ex);

      await SendAsync(ValidationFailures, 400, cancellation: c); 
    }
    catch (Exception ex)
    {
      _messageLogger.LogControllerException(
        nameof(HandleAsync),
        ex);

      await SendAsync(
        response: new { Message = "An unexpected internal server error occurred." },
        statusCode: 500,
        cancellation: c);

    }
  }
}