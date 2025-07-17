using Dapper;
using ESLAdmin.Features;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.UpdateChildcareLevel;

public class Mapper : Mapper<Request, OperationResult, DynamicParameters>
{
  public override DynamicParameters ToEntity(Request r)
  {
    DynamicParameters parameters = new DynamicParameters();

    parameters.AddInt64InputParam(
      DbConstsChildcareLevel.PARAM_CHILDCARELEVELID,
      r.ChildcareLevelId);
    parameters.AddStringInputParam(
      DbConstsChildcareLevel.PARAM_CHILDCARELEVELNAME,
      r.ChildcareLevelName,
      32);
    parameters.AddInt32InputParam(
      DbConstsChildcareLevel.PARAM_MAXCAPACITY,
      r.MaxCapacity);
    parameters.AddInt32InputParam(
      DbConstsChildcareLevel.PARAM_DISPLAYORDER,
      r.DisplayOrder);
    parameters.AddInt64InputParam(
      DbConstsChildcareLevel.PARAM_USERCODE,
      r.UserCode);
    parameters.AddStringInputParam(
      DbConstsChildcareLevel.PARAM_GUID,
      r.Guid,
      38);

    // Output parameters
    parameters.AddInt32OutputParam(
      OperationResultConsts.DBAPIERROR);
    parameters.AddStringOutputParam(
      OperationResultConsts.DUPFIELDNAME,
      128);
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
    Guid = parameters.Get<String?>(
      OperationResultConsts.GUID),
  };

}