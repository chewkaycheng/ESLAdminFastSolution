using Dapper;
using ESLAdmin.Features.ClassLevels.Endpoints.UpdateClassLevel;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Constants;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using FastEndpoints;

namespace ESLAdmin.Features.Countries.UpdateCountry;

//------------------------------------------------------------------------------
//
//                           class UpdateCountryMapper
//
//------------------------------------------------------------------------------
public class UpdateCountryMapper : Mapper<
  UpdateCountryCommand,
  OperationResult,
  DynamicParameters>
{
  //------------------------------------------------------------------------------
  //
  //                           ToParameters
  //
  //------------------------------------------------------------------------------
  public DynamicParameters ToParameters(UpdateCountryCommand command)
  {
    DynamicParameters parameters = new DynamicParameters();

    parameters.AddInt64InputParam(
      DbConstsCountry.PARAM_COUNTRY_ID,
      command.CountryId);
    parameters.AddStringInputParam(
      DbConstsCountry.PARAM_COUNTRY_NAME,
      command.CountryName,
      32);
    parameters.AddStringInputParam(
      DbConstsCountry.PARAM_LANGUAGE_NAME,
      command.LanguageName,
      32);
    parameters.AddInt64InputParam(
      DbConstsCountry.PARAM_USERCODE,
      command.UserCode);
    parameters.AddStringInputParam(
      DbConstsCountry.PARAM_GUID,
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
