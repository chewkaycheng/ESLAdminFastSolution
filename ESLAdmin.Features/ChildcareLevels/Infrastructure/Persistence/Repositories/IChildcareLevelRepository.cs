using Dapper;
using ErrorOr;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Entities;
using ESLAdmin.Infrastructure.Persistence.Entities;

namespace ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces
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
