using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Logging.Interface;
using FastEndpoints;

namespace ESLAdmin.Features.Users.Endpoints.GetUser;

//------------------------------------------------------------------------------
//
//                          class GetUserEndpoint
//
//-------------------------------------------------------------------------------
public class GetUserEndpoint : Endpoint<
  GetUserRequest, 
  APIResponse<UserResponse>, 
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
    Get("/api/users/{id}");
    AllowAnonymous();
  }

  //------------------------------------------------------------------------------
  //
  //                       HandleAsync
  //
  //-------------------------------------------------------------------------------
  public override async Task HandleAsync(
    GetUserRequest request, 
    CancellationToken cancellationToken)
  {
    try
    {
      var userResponse = await _repositoryManager.AuthenticationRepository.GetUserByIdAsync(
        request,
        Map);

      var apiResponse = new APIResponse<UserResponse>();

      if (userResponse == null)
      {
        apiResponse.IsSuccess = false;
        apiResponse.Error = $"The user with Id: {request.Id} is not found.";
        await SendAsync(apiResponse, 404, cancellationToken);
        return;
      }

      apiResponse.IsSuccess = true;
      apiResponse.Data = userResponse;
      await SendAsync(apiResponse, 201, cancellation: cancellationToken);
    }
    catch (Exception ex)
    {
      _messageLogger.LogControllerException(
        nameof(HandleAsync),
        ex);

      var apiResponse = new APIResponse<UserResponse>();
      apiResponse.IsSuccess = false;
      apiResponse.Error = "Internal Server Error.";
      await SendAsync(
        response: apiResponse,
        statusCode: 500,
        cancellation: cancellationToken);
    }
  }
}
