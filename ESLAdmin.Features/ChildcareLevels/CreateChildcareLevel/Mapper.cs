using Dapper;
using ESLAdmin.Features.ChildcareLevels.GetChildcareLevels;
using ESLAdmin.Features.ChildcareLevels.Repositories;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.CreateChildcareLevel
{
  public class Mapper : Mapper<Request, OperationResult, DynamicParameters>
  {
    public override DynamicParameters ToEntity(Request r)
    {
      DynamicParameters parameters = new DynamicParameters();
      parameters.AddStringInputParam(
        ChildcareLevelConsts.PARAM_CHILDCARELEVELNAME,
        r.ChildcareLevelName,
        32);
      parameters.AddInt32InputParam(
        ChildcareLevelConsts.PARAM_MAXCAPACITY,
        r.MaxCapacity);
      parameters.AddInt32InputParam(
        ChildcareLevelConsts.PARAM_DISPLAYORDER,
        r.DisplayOrder);
      parameters.AddInt64InputParam(
        ChildcareLevelConsts.PARAM_INITUSER,
        r.InitUser);

      // Output parameters
      parameters.AddInt32OutputParam(
        OperationResultConsts.DBAPIERROR);
      parameters.AddStringOutputParam(
        OperationResultConsts.DUPFIELDNAME,
        128);
      parameters.AddInt64OutputParam(
        ChildcareLevelConsts.CHILDCARELEVELID);
      parameters.AddStringOutputParam(
        OperationResultConsts.GUID,
        38);

      return parameters;
    }

    public override OperationResult FromEntity(DynamicParameters parameters) => new()
    {
      DbApiError = parameters.Get<Int32>(
        OperationResultConsts.DBAPIERROR),
      DupFieldName = parameters.Get<String?>(
        OperationResultConsts.DUPFIELDNAME),
      Id = parameters.Get<long?>(
        ChildcareLevelConsts.CHILDCARELEVELID),
      Guid = parameters.Get<String?>(
        OperationResultConsts.GUID),
    };
  }
}