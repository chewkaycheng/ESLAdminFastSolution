using Dapper;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Constants;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using FastEndpoints;

namespace ESLAdmin.Features.Countries.DeleteCountry;

//------------------------------------------------------------------------------
//
//                        class DeleteCountryMapper
//
//------------------------------------------------------------------------------
public class DeleteCountryMapper : Mapper<long,
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
      DbConstsCountry.PARAM_COUNTRY_ID,
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
