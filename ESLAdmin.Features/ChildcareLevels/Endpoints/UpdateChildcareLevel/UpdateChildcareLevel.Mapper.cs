using Dapper;
using ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.UpdateChildcareLevel;

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
  public DynamicParameters ToParameters(UpdateChildcareLevelCommand command)
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
      DbConstsOperationResult.DBAPIERROR);
    parameters.AddStringOutputParam(
      DbConstsOperationResult.DUPFIELDNAME,
      128);
    parameters.AddStringOutputParam(
      DbConstsOperationResult.GUID,
      38);

    return parameters;
  }

  //------------------------------------------------------------------------------
  //
  //                           FromParameters
  //
  //------------------------------------------------------------------------------
  public OperationResult FromParameters(DynamicParameters parameters) => new()
  {
    DbApiError = parameters.Get<int>(
      DbConstsOperationResult.DBAPIERROR),
    DupFieldName = parameters.Get<string?>(
      DbConstsOperationResult.DUPFIELDNAME),
    Guid = parameters.Get<string?>(
      DbConstsOperationResult.GUID),
  };
}
