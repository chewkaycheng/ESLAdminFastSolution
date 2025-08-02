using ErrorOr;
using ESLAdmin.Domain.Dtos;
using ESLAdmin.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Infrastructure.Repositories.Interfaces;

public interface IAuthenticationRepository
{
  Task<ErrorOr<User>> RegisterUserAsync(
    User user,
    string password,
    ICollection<string>? roles);

  Task<ErrorOr<UserDto>> GetUserByEmailAsync(
    string email);

  Task<ErrorOr<UserDto>> LoginAsync(string email, string password);

  Task<ErrorOr<string>> DeleteUserByEmailAsync(string email);

  Task<ErrorOr<IdentityRole>> CreateRoleAsync(string roleName);
  Task<ErrorOr<string>> UpdateRoleAsync(string oldRoleName, string newRoleName);
  Task<ErrorOr<string>> DeleteRoleAsync(string roleName);
  Task<ErrorOr<IdentityRole>> GetRoleAsync(string roleName);
  Task<ErrorOr<IEnumerable<IdentityRole>>> GetAllRolesAsync();
}
