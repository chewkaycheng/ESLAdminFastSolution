using Dapper;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.Repositories;

namespace ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces
{
  public interface IChildcareLevelRepository
  {
    Task<IEnumerable<ChildcareLevel>> GetChildcareLevelsAsync();

    Task<ChildcareLevel?> GetChildcareLevelAsync(
      DynamicParameters parameters);
    Task CreateChildcareLevelAsync(
      DynamicParameters parameters);

    Task UpdateChildcareLevelAsync(
      DynamicParameters parameters);

    Task DeleteChildcareLevelAsync(
      DynamicParameters parameters);
  }
}
