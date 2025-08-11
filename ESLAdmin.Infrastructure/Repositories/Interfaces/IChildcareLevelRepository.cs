using Dapper;
using ErrorOr;
using ESLAdmin.Domain.Entities;

namespace ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces
{
  public interface IChildcareLevelRepository
  {
    Task<ErrorOr<IEnumerable<ChildcareLevel>>> GetChildcareLevelsAsync();

    Task<ErrorOr<ChildcareLevel?>> GetChildcareLevelAsync(
      DynamicParameters parameters);
    Task<ErrorOr<bool>> CreateChildcareLevelAsync(
      DynamicParameters parameters);

    Task UpdateChildcareLevelAsync(
      DynamicParameters parameters);

    Task DeleteChildcareLevelAsync(
      DynamicParameters parameters);
  }
}
