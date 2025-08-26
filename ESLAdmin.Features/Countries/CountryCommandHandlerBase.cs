using ESLAdmin.Features.Countries.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Countries;

public abstract class CountryCommandHandlerBase<THandler>
{
  protected CountryCommandHandlerBase(
    ICountryRepository repository,
    ILogger<THandler> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  protected ICountryRepository _repository { get; }
  protected ILogger<THandler> _logger { get; }
}