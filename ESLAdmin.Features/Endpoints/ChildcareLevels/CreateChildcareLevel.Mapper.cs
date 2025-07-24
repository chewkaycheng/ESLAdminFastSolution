using Dapper;
using ESLAdmin.Infrastructure.Data.Consts;
using ESLAdmin.Infrastructure.Repositories;
using FastEndpoints;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                        Class CreateChildcareLevelMapper
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelMapper : Mapper<
  CreateChildcareLevelCommand,
  OperationResult,
  DynamicParameters>
{
  //------------------------------------------------------------------------------
  //
  //                           ToEntity
  //
  //------------------------------------------------------------------------------
  public override DynamicParameters ToEntity(CreateChildcareLevelCommand command)
  {
    DynamicParameters parameters = new DynamicParameters();
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
      DbConstsChildcareLevel.PARAM_INITUSER,
      command.InitUser);

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
