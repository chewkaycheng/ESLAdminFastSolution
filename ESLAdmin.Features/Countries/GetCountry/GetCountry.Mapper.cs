using Dapper;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Constants;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence;
using FastEndpoints;

namespace ESLAdmin.Features.Countries.GetCountry;

public class GetCountryMapper : Mapper<
  EmptyRequest,
  GetCountryResponse,
  Country>
{
  //------------------------------------------------------------------------------
  //
  //                           FromEntity 
  //
  //------------------------------------------------------------------------------
  public override GetCountryResponse FromEntity(
    Country country) => new()
  {
    Id = country.CountryId,
    CountryName = country.CountryName,
    LanguageName = country.LanguageName,
    InitUser = country.InitUser,
    InitDate = country.InitDate,
    UserCode = country.UserCode,
    UserStamp = country.UserStamp,
    Guid = country.Guid
  };
  
  //------------------------------------------------------------------------------
  //
  //                           ToParameter 
  //
  //------------------------------------------------------------------------------
  public DynamicParameters ToParameters(long id)
  {
    DynamicParameters parameters = new DynamicParameters();
    parameters.AddInt64InputParam(
      DbConstsCountry.COUNTRY_ID,
      id);
    return parameters;
  }
}