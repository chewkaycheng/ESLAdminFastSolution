namespace ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces
{
  public interface IChildcareLevelRepository
  {
    Task<APIResponse<IEnumerable<GetChildcareLevels.Response>>> GetChildcareLevels(
      GetChildcareLevels.Mapper mapper);
    Task<APIResponse<GetChildcareLevel.Response>> GetChildcareLevel(
      long id,
      GetChildcareLevel.Mapper mapper);
    Task<APIResponse<OperationResult>> CreateChildcareLevel(
      CreateChildcareLevel.Request request,
      CreateChildcareLevel.Mapper mapper);
    Task<APIResponse<OperationResult>> UpdateChildcareLevel(
      UpdateChildcareLevel.Request request,
      UpdateChildcareLevel.Mapper mapper);
    Task<APIResponse<OperationResult>> DeleteChildcareLevel(
      long id,
      DeleteChildcareLevel.Mapper mapper);
  }
}
