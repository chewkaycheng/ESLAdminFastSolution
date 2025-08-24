using Dapper;
using ErrorOr;
using ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Entities;

namespace ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Repositories
{
  public interface IChildcareLevelRepository
  {
    Task<ErrorOr<IEnumerable<ChildcareLevel>>> GetChildcareLevelsAsync();

    Task<ErrorOr<ChildcareLevel?>> GetChildcareLevelAsync(
      DynamicParameters parameters);
    Task<ErrorOr<bool>> CreateChildcareLevelAsync(
      DynamicParameters parameters);

    Task<ErrorOr<bool>> UpdateChildcareLevelAsync(
      DynamicParameters parameters);

    Task<ErrorOr<bool>> DeleteChildcareLevelAsync(
      DynamicParameters parameters);
  }
}
