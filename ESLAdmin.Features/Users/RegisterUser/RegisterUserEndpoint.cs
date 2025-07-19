using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace ESLAdmin.Features.Users.RegisterUser;

//------------------------------------------------------------------------------
//
//                       class RegisterUserEndpoint
//
//-------------------------------------------------------------------------------
public class RegisterUserEndpoint : Endpoint<
  RegisterUserRequest, APIResponse<IdentityResultEx>, RegisterUserMapper>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                       RegisterUserEndpoint
  //
  //-------------------------------------------------------------------------------
  public RegisterUserEndpoint(
    IRepositoryManager repositoryManager,
    IMessageLogger messageLogger)
  {
    _repositoryManager = repositoryManager;
    _messageLogger = messageLogger;
  }

  //------------------------------------------------------------------------------
  //
  //                       Configure
  //
  //-------------------------------------------------------------------------------
  public override void Configure()
  {
    Post("/api/register");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                       HandleAsync
  //
  //-------------------------------------------------------------------------------
  public override async Task HandleAsync(RegisterUserRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var identityResultEx = await _repositoryManager.AuthenticationRepository.RegisterUserAsync(
        request, Map);
       
      var apiResponse = new APIResponse<IdentityResultEx>();

      if (!identityResultEx.Succeeded)
      {
        foreach(var error in identityResultEx.Errors)
        {
          ValidationFailure validationFailure = new ValidationFailure(error.Code, error.Description);
          apiResponse.Errors.Add(validationFailure);
        }
        apiResponse.IsSuccess = false;
        await SendAsync(apiResponse, 400, cancellation: cancellationToken);
        return;
      }

      HttpContext.Response.Headers.Append(
        "location", $"/api/GetUser/{identityResultEx.Id}");

      apiResponse.IsSuccess = true;
      apiResponse.Data = identityResultEx;
      await SendAsync(apiResponse, 201, cancellation: cancellationToken);
      //await SendCreatedAtAsync<ReigsterUserEndpoint>(
      //  routeValues: new { id = response.Data.User.Id },
      //  new EmptyResponse(), 
      //  cancellation: c);
    }
    //catch (ValidationFailureException ex)
    //{
    //  _messageLogger.LogControllerException(
    //    nameof(HandleAsync),
    //    ex);


    //  await SendAsync(ValidationFailures, 400, cancellation: cancellationToken); 
    //}
    catch (Exception ex)
    {
      _messageLogger.LogControllerException(
        nameof(HandleAsync),
        ex);

      var apiResponse = new APIResponse<IdentityResultEx>();
      apiResponse.IsSuccess = false;
      apiResponse.Error = "Internal Server Error.";
      
      await SendAsync(
        response: apiResponse,
        statusCode: 500,
        cancellation: cancellationToken);

    }
  }
}