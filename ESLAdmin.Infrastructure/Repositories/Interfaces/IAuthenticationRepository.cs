using ESLAdmin.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

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

  Task<IdentityResultEx> CreateRoleAsync(string roleName);
  Task<IdentityResultEx> UpdateRoleAsync(string oldRoleName, string newRoleName);
  Task<IdentityResultEx> DeleteRoleAsync(string roleName);
  Task<IdentityRole?> GetRoleAsync(string roleName);
  Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
}
