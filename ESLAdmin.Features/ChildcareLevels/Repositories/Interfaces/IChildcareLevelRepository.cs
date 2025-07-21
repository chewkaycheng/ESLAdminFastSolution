using ESLAdmin.Features.ChildcareLevels.Endpoints.CreateChildcareLevel;
using ESLAdmin.Features.ChildcareLevels.Endpoints.DeleteChildcareLevel;
using ESLAdmin.Features.ChildcareLevels.Endpoints.UpdateChildcareLevel;
using ESLAdmin.Features.ChildcareLevels.Mappers;
using ESLAdmin.Features.ChildcareLevels.Models;

namespace ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces
{
  public interface IChildcareLevelRepository
  {
    Task<IEnumerable<ChildcareLevelResponse>> GetChildcareLevels(
      ChildcareLevelMapper mapper);
    Task<ChildcareLevelResponse> GetChildcareLevel(
      long id,
      ChildcareLevelMapper mapper);
    Task<OperationResult> CreateChildcareLevelAsync(
      CreateChildcareLevelRequest request,
      CreateChildcareLevelMapper mapper);
    Task<OperationResult> UpdateChildcareLevelAsync(
      UpdateChildcareLevelRequest request,
      UpdateChildcareLevelMapper mapper);
    Task<OperationResult> DeleteChildcareLevel(
      long id,
      DeleteChildcareLevelMapper mapper);
  }
}
