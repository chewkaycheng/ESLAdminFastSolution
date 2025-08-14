using ESLAdmin.Common.Errors;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Endpoints.Roles;
//-------------------------------------------------------------------------------
//
//                       class GetRoleCommandHandler
//
//-------------------------------------------------------------------------------
public class GetRoleCommandHandler : ICommandHandler<
  GetRoleCommand,
  Results<
    Ok<GetRoleResponse>,
    ProblemDetails,
    InternalServerError>>
{
  private readonly IRepositoryManager _repositoryManager;
  private readonly ILogger<GetRoleCommandHandler> _logger;

  //-------------------------------------------------------------------------------
  //
  //                       GetRoleCommandHandler
  //
  //-------------------------------------------------------------------------------
  public GetRoleCommandHandler(
    IRepositoryManager repositoryManager,
    ILogger<GetRoleCommandHandler> logger)
  {
    _repositoryManager = repositoryManager;
    _logger = logger;
  }


  //-------------------------------------------------------------------------------
  //
  //                       ExecuteAsync
  //
  //-------------------------------------------------------------------------------
  public async Task<Results<Ok<GetRoleResponse>, ProblemDetails, InternalServerError>>
    ExecuteAsync(
      GetRoleCommand command,
      CancellationToken ct)
  {
    try
    {
      var result = await _repositoryManager.IdentityRepository.GetRoleAsync(command.Name);
      if (result.IsError)
      {
        _logger.LogNotFound("role", $"name: '{command.Name}'");
        var error = result.Errors.First();
        return new ProblemDetails(
           ErrorUtils.CreateFailureList(
             error.Code,
             error.Description),
           StatusCodes.Status404NotFound);
      }

      var response = command.Mapper.FromEntity(result.Value);
      return TypedResults.Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return TypedResults.InternalServerError();
    }
  }
}
