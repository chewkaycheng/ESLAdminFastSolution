using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

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
  //                       HandleAsync
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
        return TypedResults.UnprocessableEntity(errors);
      }

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
        nameof(HandleAsync),
        ex);

      return TypedResults.InternalServerError();

    }
  }
}