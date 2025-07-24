using Dapper;
using ESLAdmin.Infrastructure.Repositories;

namespace ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces
{
  public interface IChildcareLevelRepository
  {
    //Task<IEnumerable<ChildcareLevelResponse>> GetChildcareLevels(
    //  ChildcareLevelMapper mapper);
    //Task<ChildcareLevelResponse> GetChildcareLevel(
    //  long id,
    //  ChildcareLevelMapper mapper);
    Task CreateChildcareLevelAsync(
      DynamicParameters parameters);
    //Task<OperationResult> UpdateChildcareLevelAsync(
    //  UpdateChildcareLevelCommand command);
    //Task<OperationResult> DeleteChildcareLevel(
    //  long id,
    //  DeleteChildcareLevelMapper mapper);
  }
}
