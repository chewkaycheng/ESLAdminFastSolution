using Dapper;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.CreateChildcareLevel;

//------------------------------------------------------------------------------
//
//                        Class CreateChildcareLevelMapper
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelMapper : Mapper<
  CreateChildcareLevelRequest, 
  OperationResult, 
  DynamicParameters>
{
  //------------------------------------------------------------------------------
  //
  //                           ToEntity
  //
  //------------------------------------------------------------------------------
  public override DynamicParameters ToEntity(CreateChildcareLevelRequest r)
  {
    DynamicParameters parameters = new DynamicParameters();
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
      DbConstsChildcareLevel.PARAM_INITUSER,
      r.InitUser);

    // Output parameters
    parameters.AddInt32OutputParam(
      OperationResultConsts.DBAPIERROR);
    parameters.AddStringOutputParam(
      OperationResultConsts.DUPFIELDNAME,
      128);
    parameters.AddInt64OutputParam(
      DbConstsChildcareLevel.CHILDCARELEVELID);
    parameters.AddStringOutputParam(
      OperationResultConsts.GUID,
      38);

    return parameters;
  }

  //------------------------------------------------------------------------------
  //
  //                           FromEntity
  //
  //------------------------------------------------------------------------------
  public override OperationResult FromEntity(
    DynamicParameters parameters) => new()
  {
    DbApiError = parameters.Get<int>(
      OperationResultConsts.DBAPIERROR),
    DupFieldName = parameters.Get<string?>(
      OperationResultConsts.DUPFIELDNAME),
    Id = parameters.Get<long?>(
      DbConstsChildcareLevel.CHILDCARELEVELID),
    Guid = parameters.Get<string?>(
      OperationResultConsts.GUID),
  };
}