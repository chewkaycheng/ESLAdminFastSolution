using ESLAdmin.Features.Users.Endpoints.GetUser;
using ESLAdmin.Features.Users.Endpoints.RegisterUser;
using ESLAdmin.Features.Users.Models;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Features.Users.Repositories.Interfaces;

public interface IAuthenticationRepository
{
  Task<IdentityResultEx> RegisterUserAsync(
    RegisterUserRequest request,
    RegisterUserMapper mapper);

  Task<UserResponse> GetUserByEmailAsync(
    GetUserRequest request,
    GetUserMapper mapper);
}
