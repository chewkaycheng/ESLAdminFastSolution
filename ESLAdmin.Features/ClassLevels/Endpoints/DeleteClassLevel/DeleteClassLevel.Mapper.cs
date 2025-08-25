using Dapper;
using ESLAdmin.Features.ClassLevels.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using FastEndpoints;

namespace ESLAdmin.Features.ClassLevels.Endpoints.DeleteClassLevel
{
  public class DeleteClassLevelMapper : Mapper<long, 
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
        DbConstsClassLevel.PARAM_CLASSLEVELID,
        id);

      // Output parameters
      parameters.AddStringOutputParam(
        DbConstsOperationResult.REFERENCETABLE,
        64);
      parameters.AddInt32OutputParam(
        DbConstsOperationResult.DBAPIERROR);

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
          DbConstsOperationResult.DBAPIERROR),
      ReferenceTable = parameters.Get<string?>(
          DbConstsOperationResult.REFERENCETABLE),
    };
  }
}
