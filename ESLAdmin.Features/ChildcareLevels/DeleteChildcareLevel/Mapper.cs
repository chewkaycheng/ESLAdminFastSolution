using Dapper;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.DeleteChildcareLevel;

public class Mapper : Mapper<long, OperationResult, DynamicParameters>
{
  public override DynamicParameters ToEntity(long id)
  {
    DynamicParameters parameters = new DynamicParameters();
    parameters.AddInt64InputParam(
      DbConstsChildcareLevel.PARAM_CHILDCARELEVELID,
      id);

    // Output parameters
    parameters.AddStringOutputParam(
      OperationResultConsts.REFERENCETABLE,
      64);
    parameters.AddInt32OutputParam(
      OperationResultConsts.DBAPIERROR);
   
    return parameters;
  }

  public override OperationResult FromEntity(DynamicParameters parameters) => new()
  {
    DbApiError = parameters.Get<Int32>(
        OperationResultConsts.DBAPIERROR),
    ReferenceTable = parameters.Get<String?>(
        OperationResultConsts.REFERENCETABLE),
  };
}