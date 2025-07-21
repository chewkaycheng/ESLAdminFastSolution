using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ESLAdmin.Features.Users.Endpoints.RegisterUser;

//------------------------------------------------------------------------------
//
//                       class RegisterUserEndpoint
//
//-------------------------------------------------------------------------------
public class RegisterUserEndpoint : Endpoint<
  RegisterUserRequest,
  Results<NoContent,
    UnprocessableEntity<APIErrors>,
    InternalServerError>,
  RegisterUserMapper>
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
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public override async
    Task<Results<NoContent,
      UnprocessableEntity<APIErrors>,
      InternalServerError>>
    ExecuteAsync(
      RegisterUserRequest request,
      CancellationToken cancellationToken)
  {
    var Roles = request.Roles != null ? string.Join(", ", request.Roles) : "None";
    var requestLog = JsonSerializer.Serialize(new
    {
      request.UserName,
      request.FirstName,
      request.LastName,
      request.Email,
      Password = "[Hidden]",
      request.PhoneNumber,
      Roles = request.Roles != null ? string.Join(", ", request.Roles) : "None"
    });
    var roleLog = Roles = request.Roles != null ? string.Join(", ", request.Roles) : "None";
    _messageLogger.Logger.LogDebug($"Invocation. \n=>Request: \n    Username: {request.UserName}, FirstName: {request.FirstName}, LastName: {request.LastName}, Email: {request.Email}\n    Password: '[Hidden]', PhoneNumber: {request.PhoneNumber}, Roles: {roleLog}");
    
    try
    {
      
      var identityResultEx = await _repositoryManager.AuthenticationRepository.RegisterUserAsync(
      request, Map);

      if (!identityResultEx.Succeeded)
      {
        ValidationFailures.AddRange(
          identityResultEx.Errors.Select(error => new ValidationFailure
          {
            PropertyName = error.Code,
            ErrorMessage = error.Description
          })); 
        APIErrors errors = new APIErrors();
        errors.Errors = ValidationFailures;

        var errorList = "";
        foreach (var error in identityResultEx.Errors)
        {
          errorList += $"\n    Code: {error.Code}, Description: {error.Description}";
        }

        _messageLogger.Logger.LogDebug($"Validation errors.\nErrors:{errorList}");
        return TypedResults.UnprocessableEntity(errors);
      }

      _messageLogger.Logger.LogDebug($"Success. User Id: {identityResultEx.Id}");

      HttpContext.Response.Headers.Append(
        "location", $"/api/users/{request.Email}");

      return TypedResults.NoContent();

      //await SendCreatedAtAsync<ReigsterUserEndpoint>(
      //  routeValues: new { id = response.Data.User.Id },
      //  new EmptyResponse(), 
      //  cancellation: c);
    }
    catch (Exception ex)
    {
      _messageLogger.LogControllerException(
        nameof(ExecuteAsync),
        ex);

      return TypedResults.InternalServerError();

    }
  }
}