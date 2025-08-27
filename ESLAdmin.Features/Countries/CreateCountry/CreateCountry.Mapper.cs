using Dapper;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using FastEndpoints;

namespace ESLAdmin.Features.Countries.CreateCountry;

//------------------------------------------------------------------------------
//
//                        Class CreateCountryMapper
//
//------------------------------------------------------------------------------
public class CreateCountryMapper : Mapper<
  CreateCountryCommand,
  OperationResult,
  DynamicParameters>
{
  //------------------------------------------------------------------------------
  //
  //                           ToParameters
  //
  //------------------------------------------------------------------------------
  public DynamicParameters ToParameters(
    CreateCountryCommand command)
  {
    DynamicParameters parameters = new DynamicParameters();
    parameters.AddStringInputParam(
      DbConstsCountry.PARAM_COUNTRY_NAME,
      command.CountryName,
      32);
    parameters.AddStringInputParam(
      DbConstsCountry.PARAM_LANGUAGE_NAME,
      command.LanguageName,
      32);
    parameters.AddInt64InputParam(
      DbConstsCountry.PARAM_INITUSER,
      command.InitUser);

    // Output parameters
    parameters.AddInt32OutputParam(
      DbConstsOperationResult.DBAPIERROR);
    parameters.AddStringOutputParam(
      DbConstsOperationResult.DUPFIELDNAME,
      128);
    parameters.AddInt64OutputParam(
      DbConstsCountry.COUNTRY_ID);
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
  public OperationResult FromParameters(
    DynamicParameters parameters) => new()
    {
      DbApiError = parameters.Get<int>(
      DbConstsOperationResult.DBAPIERROR),
      DupFieldName = parameters.Get<string?>(
      DbConstsOperationResult.DUPFIELDNAME),
      Id = parameters.Get<long?>(
      DbConstsCountry.COUNTRY_ID),
      Guid = parameters.Get<string?>(
      DbConstsOperationResult.GUID),
    };

  //------------------------------------------------------------------------------
  //
  //                           ToResponse
  //
  //------------------------------------------------------------------------------
  public CreateCountryResponse ToResponse(DynamicParameters parameters)
  {
    return new CreateCountryResponse
    {
      CountryId = parameters.Get<long>(
        DbConstsCountry.COUNTRY_ID),
      Guid = parameters.Get<string>(
        DbConstsOperationResult.GUID)
    };
  }
}