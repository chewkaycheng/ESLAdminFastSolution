using Dapper;
using ESLAdmin.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence.Repositories;
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
  //                        ToParameters
  //
  //------------------------------------------------------------------------------
  public DynamicParameters ToParameters(long id)
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
  //                        FromParameters
  //
  //------------------------------------------------------------------------------
  public OperationResult FromParameters(DynamicParameters parameters) => new()
  {
    DbApiError = parameters.Get<int>(
        OperationResultConsts.DBAPIERROR),
    ReferenceTable = parameters.Get<string?>(
        OperationResultConsts.REFERENCETABLE),
  };
}