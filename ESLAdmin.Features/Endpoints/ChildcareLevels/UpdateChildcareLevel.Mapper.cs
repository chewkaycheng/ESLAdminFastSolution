using Dapper;
using ESLAdmin.Infrastructure.Data.Consts;
using ESLAdmin.Infrastructure.Repositories;
using FastEndpoints;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                           ToEntity
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelMapper : Mapper<
  UpdateChildcareLevelCommand,
  OperationResult,
  DynamicParameters>
{
  public override DynamicParameters ToEntity(UpdateChildcareLevelCommand command)
  {
    DynamicParameters parameters = new DynamicParameters();

    parameters.AddInt64InputParam(
      DbConstsChildcareLevel.PARAM_CHILDCARELEVELID,
      command.ChildcareLevelId);
    parameters.AddStringInputParam(
      DbConstsChildcareLevel.PARAM_CHILDCARELEVELNAME,
      command.ChildcareLevelName,
      32);
    parameters.AddInt32InputParam(
      DbConstsChildcareLevel.PARAM_MAXCAPACITY,
      command.MaxCapacity);
    parameters.AddInt32InputParam(
      DbConstsChildcareLevel.PARAM_DISPLAYORDER,
      command.DisplayOrder);
    parameters.AddInt64InputParam(
      DbConstsChildcareLevel.PARAM_USERCODE,
      command.UserCode);
    parameters.AddStringInputParam(
      DbConstsChildcareLevel.PARAM_GUID,
      command.Guid,
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

  //------------------------------------------------------------------------------
  //
  //                           FromEntity
  //
  //------------------------------------------------------------------------------
  public override OperationResult FromEntity(DynamicParameters parameters) => new()
  {
    DbApiError = parameters.Get<int>(
      OperationResultConsts.DBAPIERROR),
    DupFieldName = parameters.Get<string?>(
      OperationResultConsts.DUPFIELDNAME),
    Guid = parameters.Get<string?>(
      OperationResultConsts.GUID),
  };
}
