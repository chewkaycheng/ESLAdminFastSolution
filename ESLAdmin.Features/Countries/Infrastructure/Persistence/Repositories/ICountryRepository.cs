using Dapper;
using ErrorOr;
using ESLAdmin.Features.Countries.Infrastructure.Persistence.Entities;

namespace ESLAdmin.Features.Countries.Infrastructure.Persistence.Repositories;

public interface ICountryRepository
{
  Task<ErrorOr<IEnumerable<Country>>> GetCountriesAsync();

  Task<ErrorOr<Country?>> GetCountryAsync(
    DynamicParameters parameters);

  Task<ErrorOr<Success>> CreateCountryAsync(
    DynamicParameters parameters);
  
  Task<ErrorOr<Success>> UpdateCountryAsync(
    DynamicParameters parameters);

  Task<ErrorOr<Success>> DeleteCountryAsync(
    DynamicParameters parameters);
}