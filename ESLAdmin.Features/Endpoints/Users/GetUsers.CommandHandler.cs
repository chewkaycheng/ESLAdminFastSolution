using ESLAdmin.Domain.Dtos;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Endpoints.ChildcareLevels;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                        class GetUsersCommandHandler
//
//------------------------------------------------------------------------------
public class GetUsersCommandHandler :
  ICommandHandler<
    GetUsersCommand,
    Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetUserCommandHandler> _logger;

  //------------------------------------------------------------------------------
  //
  //                        GetUsersCommandHandler
  //
  //------------------------------------------------------------------------------
  public GetUsersCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<GetUserCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }

  //------------------------------------------------------------------------------
  //
  //                        ExecuteAsync
  //
  //------------------------------------------------------------------------------
  public async Task<Results<Ok<IEnumerable<GetUserResponse>>, ProblemDetails, InternalServerError>> ExecuteAsync(GetUsersCommand command, CancellationToken ct)
  {
    try
    {
      var userDtos = await _repositoryManager.AuthenticationRepository.GetAllUsersAsync();

      IEnumerable<GetUserResponse> usersResponse =
       userDtos.Select(
         userDto => command.Mapper.DtoToResponse(
           userDto)).ToList();
     
      return TypedResults.Ok(usersResponse);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}
