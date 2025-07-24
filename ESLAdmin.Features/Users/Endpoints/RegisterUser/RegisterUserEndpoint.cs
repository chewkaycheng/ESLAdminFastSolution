using ESLAdmin.Infrastructure.RepositoryManagers;
using ESLAdmin.Logging;
using ESLAdmin.Logging.Interface;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Users.Endpoints.RegisterUser;

////------------------------------------------------------------------------------
////
////                       class RegisterUserEndpoint
////
////-------------------------------------------------------------------------------
//public class RegisterUserEndpoint : Endpoint<
//  RegisterUserRequest,
//  Results<NoContent,
//    ProblemDetails,
//    InternalServerError>,
//  RegisterUserMapper>
//{
//  private readonly IRepositoryManager _repositoryManager;
//  private readonly IMessageLogger _messageLogger;
//  private readonly ILogger<RegisterUserEndpoint> _logger;

//  //------------------------------------------------------------------------------
//  //
//  //                       RegisterUserEndpoint
//  //
//  //-------------------------------------------------------------------------------
//  public RegisterUserEndpoint(
//    IRepositoryManager repositoryManager,
//    ILogger<RegisterUserEndpoint> logger,
//    IMessageLogger messageLogger)
//  {
//    _repositoryManager = repositoryManager;
//    _logger = logger;
//    _messageLogger = messageLogger;
//  }

//  //------------------------------------------------------------------------------
//  //
//  //                       Configure
//  //
//  //-------------------------------------------------------------------------------
//  public override void Configure()
//  {
//    Post("/api/register");
//    AllowAnonymous();
//  }

//  //------------------------------------------------------------------------------
//  //
//  //                       ExecuteAsync
//  //
//  //-------------------------------------------------------------------------------
//  public override async
//    Task<Results<NoContent,
//      ProblemDetails,
//      InternalServerError>>
//    ExecuteAsync(
//      RegisterUserRequest request,
//      CancellationToken cancellationToken)
//  {
//    var Roles = request.Roles != null ? string.Join(", ", request.Roles) : "None";

//    if (_logger.IsEnabled(LogLevel.Debug))
//    {
//      var roleLog = Roles = request.Roles != null ? string.Join(", ", request.Roles) : "None";
//      var context = $"\n=>Request: \n    Username: '{request.UserName}', FirstName: '{request.FirstName}', LastName: '{request.LastName}', Email: '{request.Email}'\n    Password: '[Hidden]', PhoneNumber: '{request.PhoneNumber}', Roles: '{roleLog}'";
//      _logger.LogFunctionEntry(context);
//    }
//    var command = new RegisterUserCommand
//    {
//      UserName = request.UserName,
//      FirstName = request.FirstName,
//      LastName = request.LastName,
//      Email = request.Email,
//      Password = request.Password,
//      PhoneNumber = request.PhoneNumber,
//      Roles = request.Roles,
//      Mapper = Map
//    };

//    var result = await command.ExecuteAsync(cancellationToken);

//    if (result.Result is NoContent)
//    {
//      HttpContext.Response.Headers.Append("location", $"/api/users/{request.Email}");
//    }

//    return result;
//  }
//}