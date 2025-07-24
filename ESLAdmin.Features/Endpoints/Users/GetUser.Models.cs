using ESLAdmin.Features.Users.Endpoints.GetUser;
using ESLAdmin.Features.Users.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.Endpoints.Users;

//------------------------------------------------------------------------------
//
//                       class GetUserRequest
//
//-------------------------------------------------------------------------------
public class GetUserRequest
{
  public string Email { get; set; }
}

//------------------------------------------------------------------------------
//
//                       class GetUserResponse
//
//-------------------------------------------------------------------------------
public class GetUserResponse
{
  public string Id { get; init; }
  public string? FirstName { get; init; }
  public string? LastName { get; init; }
  public required string UserName { get; init; }
  public required string Email { get; init; }
  public string? PhoneNumber { get; init; }
  public ICollection<string>? Roles { get; init; }
}
