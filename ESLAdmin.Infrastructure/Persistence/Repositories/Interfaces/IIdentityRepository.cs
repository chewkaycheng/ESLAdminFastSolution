using ErrorOr;
using ESLAdmin.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;

public interface IIdentityRepository
{
  Task<ErrorOr<Success>> AddToRoleAsync(
  string email,
  string roleName);
  Task<ErrorOr<Success>> DeleteUserByEmailAsync(string email);
  Task<ErrorOr<User>> GetUserByEmailAsync(
  string email);
  Task<ErrorOr<List<string>>> GetRolesForUserAsync(
    User user);
  Task<ErrorOr<IEnumerable<User>>> GetAllUsersAsync();
  Task<ErrorOr<IEnumerable<UserRole>>> GetAllUserRolesAsync();
  Task<ErrorOr<string>> RemoveFromRoleAsync(
  string email,
  string roleName);
  Task<ErrorOr<User>> LoginAsync(string email, string password);
  Task<ErrorOr<User>> RegisterUserAsync(
    User user,
    string password,
    ICollection<string>? roles);
  Task<ErrorOr<RefreshToken>> ValidateRefreshTokenAsync(
    string token,
    CancellationToken cancellationToken = default);
  Task<ErrorOr<string>> GenerateRefreshTokenAsync(
    string userId, 
    CancellationToken cancellationToken = default);
  Task<ErrorOr<RefreshToken>> GetRefreshTokenAsync(string token);
  Task<ErrorOr<Success>> RevokeRefreshTokenAsync(string token);
  Task<ErrorOr<Success>> RevokeRefreshTokenAsync(
    string userId,
    CancellationToken cancellationToken = default);
  Task<ErrorOr<User>> FindByIdAsync(string userId);
  Task<ErrorOr<User>> FindByUserNameAsync(string username);

  Task<ErrorOr<Success>> AddRefreshTokenAsync(RefreshToken refreshToken);
  Task<ErrorOr<IdentityRole>> CreateRoleAsync(string roleName);
  Task<ErrorOr<string>> UpdateRoleAsync(string oldRoleName, string newRoleName);
  Task<ErrorOr<string>> DeleteRoleAsync(string roleName);
  Task<ErrorOr<IdentityRole>> GetRoleAsync(string roleName);
  Task<ErrorOr<IEnumerable<IdentityRole>>> GetAllRolesAsync();
  Task<ErrorOr<List<string>>> GetRolesAsync(User user);
}
