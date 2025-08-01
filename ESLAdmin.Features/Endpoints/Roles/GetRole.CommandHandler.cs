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
      var result = await _repositoryManager.AuthenticationRepository.GetRoleAsync(command.Name);
      if (result.IsError)
      {
        foreach (var error in result.Errors)
        {
          if (error.Code == "Exception")
            return TypedResults.InternalServerError();

          var validationFailures = new List<ValidationFailure>();
          validationFailures.AddRange(new ValidationFailure
          {
            PropertyName = error.Code,
            ErrorMessage = error.Description
          });
          return new ProblemDetails(validationFailures, StatusCodes.Status404NotFound);
        }
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
