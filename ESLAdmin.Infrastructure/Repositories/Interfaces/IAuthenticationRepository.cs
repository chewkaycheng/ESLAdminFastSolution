using ErrorOr;
using ESLAdmin.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Infrastructure.Repositories.Interfaces;

public interface IAuthenticationRepository
{
  Task<IdentityResultEx> RegisterUserAsync(
    User user,
    string password,
    ICollection<string>? roles);

  Task<(User user, ICollection<string>? roles)?> GetUserByEmailAsync(
    string email);

  Task<(User user, ICollection<string>? roles)?> LoginAsync(string email, string password);

  Task<IdentityResultEx> DeleteUserByEmailAsync(string email);

  Task<ErrorOr<IdentityRole>> CreateRoleAsync(string roleName);
  Task<ErrorOr<string>> UpdateRoleAsync(string oldRoleName, string newRoleName);
  Task<ErrorOr<string>> DeleteRoleAsync(string roleName);
  Task<ErrorOr<IdentityRole>> GetRoleAsync(string roleName);
  Task<ErrorOr<IEnumerable<IdentityRole>>> GetAllRolesAsync();
}
