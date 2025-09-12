using Dapper;
using ErrorOr;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Constants;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Entities;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Countries.Infrastructure.Persistence.Repositories;

//------------------------------------------------------------------------------
//
//                        class CountryRepository
//
//------------------------------------------------------------------------------
public class CountryRepository :
  RepositoryBase<Country, OperationResult>,
  ICountryRepository
{
  public CountryRepository(
    IDbContextDapper dbContextDapper, 
    ILogger<CountryRepository> logger) : 
    base(dbContextDapper, logger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        GetCountriesAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<IEnumerable<Country>>> GetCountriesAsync()
  {
    var sql = DbConstsCountry.SQL_GET_ALL;
    return await GetAllAsync(sql);
  }

  //------------------------------------------------------------------------------
  //
  //                        GetCountryAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<Country?>> GetCountryAsync(
    DynamicParameters parameters)
  {
    var country =  await GetSingleAsync(
      DbConstsCountry.SQL_GET_BY_ID, 
      parameters);
    return country;
  }

  public async Task<ErrorOr<Success>> CreateCountryAsync(DynamicParameters parameters)
  {
    return await PersistAsync(
      DbConstsCountry.SP_COUNTRY_ADD,
      parameters);
  }

  public async Task<ErrorOr<Success>> UpdateCountryAsync(DynamicParameters parameters)
  {
    return await PersistAsync(
      DbConstsCountry.SP_COUNTRY_UPD,
      parameters);
  }

  public async Task<ErrorOr<Success>> DeleteCountryAsync(DynamicParameters parameters)
  {
    return await PersistAsync(
      DbConstsCountry.SP_COUNTRY_DEL,
      parameters);
  }
}