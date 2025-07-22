using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Users.Endpoints.GetUser;

//------------------------------------------------------------------------------
//
//                          class GetUserEndpoint
//
//-------------------------------------------------------------------------------
public class GetUserEndpoint : Endpoint<
  GetUserRequest,
  Results<Ok<UserResponse>,
    ProblemDetails,
    InternalServerError>,
  GetUserMapper>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly IMessageLogger _messageLogger;

  //------------------------------------------------------------------------------
  //
  //                       GetUserEndpoint
  //
  //-------------------------------------------------------------------------------
  public GetUserEndpoint(
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
    Get("/api/users/{email}");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public override async Task<Results<Ok<UserResponse>, ProblemDetails, InternalServerError>> ExecuteAsync(
    GetUserRequest request, 
    CancellationToken cancellationToken)
  {
    try
    {
                  
      var userResponse = await _repositoryManager.AuthenticationRepository.GetUserByEmailAsync(
        request,
        Map);

      var apiResponse = new APIResponse<UserResponse>();

      if (userResponse == null)
      {
        APIErrors errors = new APIErrors();
        ValidationFailures.AddRange(new ValidationFailure
        {
          PropertyName = "NotFound",
          ErrorMessage = $"The user with email: {request.Email} is not found."
        });
        return new ProblemDetails(
          ValidationFailures, 
          StatusCodes.Status404NotFound);
      }

      return TypedResults.Ok(userResponse);
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
