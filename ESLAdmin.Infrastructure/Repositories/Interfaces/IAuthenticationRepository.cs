using ESLAdmin.Domain.Entities;
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
}
