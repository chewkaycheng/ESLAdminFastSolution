using ErrorOr;
using ESLAdmin.Domain.Dtos;
using ESLAdmin.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading;

namespace ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;

public interface IIdentityRepository
{
  Task<ErrorOr<string>> AddToRoleAsync(
  string email,
  string roleName);
  Task<ErrorOr<string>> DeleteUserByEmailAsync(string email);
  Task<ErrorOr<UserDto>> GetUserByEmailAsync(
  string email);
  Task<IEnumerable<UserDto>> GetAllUsersAsync();
  Task<ErrorOr<string>> RemoveFromRoleAsync(
  string email,
  string roleName);
  Task<ErrorOr<UserDto>> LoginAsync(string email, string password);
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
  Task<ErrorOr<bool>> RevokeRefreshTokenAsync(string token);
  Task<ErrorOr<Success>> RevokeRefreshTokenAsync(
    string userId,
    CancellationToken cancellationToken = default);
  Task<ErrorOr<User>> FindByIdAsync(string userId);
  Task<ErrorOr<User>> FindByUserNameAsync(string username);

  Task AddRefreshTokenAsync(RefreshToken refreshToken);
  Task<ErrorOr<IdentityRole>> CreateRoleAsync(string roleName);
  Task<ErrorOr<string>> UpdateRoleAsync(string oldRoleName, string newRoleName);
  Task<ErrorOr<string>> DeleteRoleAsync(string roleName);
  Task<ErrorOr<IdentityRole>> GetRoleAsync(string roleName);
  Task<ErrorOr<IEnumerable<IdentityRole>>> GetAllRolesAsync();
  Task<List<string>> GetRolesAsync(User user);
}
