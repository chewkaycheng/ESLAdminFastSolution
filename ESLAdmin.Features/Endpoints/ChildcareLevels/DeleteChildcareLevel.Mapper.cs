using Dapper;
using ESLAdmin.Infrastructure.Data.Consts;
using ESLAdmin.Infrastructure.Repositories;
using FastEndpoints;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                        class DeleteChildcareLevelMapper
//
//------------------------------------------------------------------------------
public class DeleteChildcareLevelMapper : Mapper<
  long,
  OperationResult,
  DynamicParameters>
{
  //------------------------------------------------------------------------------
  //
  //                        ToEntity
  //
  //------------------------------------------------------------------------------
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

  //------------------------------------------------------------------------------
  //
  //                        FromEntity
  //
  //------------------------------------------------------------------------------
  public override OperationResult FromEntity(DynamicParameters parameters) => new()
  {
    DbApiError = parameters.Get<int>(
        OperationResultConsts.DBAPIERROR),
    ReferenceTable = parameters.Get<string?>(
        OperationResultConsts.REFERENCETABLE),
  };
}