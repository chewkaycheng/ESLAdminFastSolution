using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Endpoints;

public abstract class IdentityCommandHandlerBase<THandler>
{
  protected IdentityCommandHandlerBase(
    IIdentityRepository repository,
    ILogger<THandler> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  protected IIdentityRepository _repository { get; }
  protected ILogger<THandler> _logger { get; }
}