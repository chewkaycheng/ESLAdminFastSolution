using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging.Interface;

namespace ESLAdmin.Features.Endpoints.Users;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand,
    Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILogger<LoginUserCommandHandler> _logger;
    private readonly IMessageLogger _messageLogger;

    public LoginUserCommandHandler(
        IRepositoryManager repositoryManager,
        ILogger<LoginUserCommandHandler> logger,
        IMessageLogger messageLogger)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
        _messageLogger = messageLogger;
    }

    public async Task<Results<Ok<LoginUserResponse>, ProblemDetails, InternalServerError>>
        ExecuteAsync(
            LoginUserCommand command,
            CancellationToken cancellationToken)
    {
        try
        {
            var result = await _repositoryManager.AuthenticationRepository.GetUserByEmailAsync(
                command.Email);

            switch (result)
            {
                case null:
                    var validationFailures = new List<ValidationFailure>();
                    validationFailures.AddRange(new ValidationFailure
                    {
                        PropertyName = "NotFound",
                        Description = "The user with email: {command.Email} is not found."
                    });
                    LogValidationErrors(validationFailures);
                    return new ProblemDetails(
                        validationFailures,
                        StatusCodes.Status404NotFound);
                default:
                    var (user, roles) = result.Value;
                    var userResponse = command.Mapper.ToResponse(user, roles?.ToList());

                    return TypedResults.Ok(userResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
            //_messageLogger.LogControllerException(
            //  nameof(ExecuteAsync),
            //  ex);

            return TypedResults.InternalServerError();
        }
    }
    private void LogValidationErrors(List<ValidationFailure> validationFailures)
    {
        var validationErrorsStr = "";
        foreach (var validationFailure in validationFailures)
        {
            validationErrorsStr += $"\n    PropertyName: {validationFailure.PropertyName}, ErrorMessage: {validationFailure.ErrorMessage}";
        }
        _logger.LogValidationErrors(validationErrorsStr);
    }

}


public class LoginUserResponse
{
}