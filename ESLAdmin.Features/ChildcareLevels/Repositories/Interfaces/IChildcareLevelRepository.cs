using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.CreateChildcareLevel;
namespace ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces
{
  public interface IChildcareLevelRepository
  {
    Task<IEnumerable<ChildcareLevel>> GetAllChildcareLevelsAsync();
    Task<ChildcareLevel?> GetChildcareLevelByIdAsync(long id);
    Task<OperationResult> CreateChildcareLevelAsync(
      CreateChildcareLevel.Mapper mapper,
      CreateChildcareLevel.Request request);
    Task<OperationResult> UpdateChildcareLevelAsync(
      long id,
      UpdateChildcareLevel.Request request);
    Task<OperationResult> DeleteChildcareLevelAsync(
      long id);
    Task CreateChildcareLevelAsync(object mapper, Request r);
  }
}
