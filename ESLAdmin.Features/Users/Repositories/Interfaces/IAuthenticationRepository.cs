using ESLAdmin.Features.Users.Endpoints.GetUser;
using ESLAdmin.Features.Users.Models;
using ESLAdmin.Features.Users.RegisterUser;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Features.Users.Repositories.Interfaces;

public interface IAuthenticationRepository
{
  Task<IdentityResultEx> RegisterUserAsync(
    RegisterUserRequest request,
    RegisterUserMapper mapper);

  Task<UserResponse> GetUserByIdAsync(
    GetUserRequest request,
    GetUserMapper mapper);
}
