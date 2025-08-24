using ESLAdmin.Features.ClassLevels.Infrastructure.Persistence.Repositories;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ClassLevels.Endpoints;

public abstract class ClassLevelCommandHandlerBase<THandler>
{
  protected ClassLevelCommandHandlerBase(
    IClassLevelRepository repository,
    ILogger<THandler> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  protected IClassLevelRepository _repository { get; }
  protected ILogger<THandler> _logger { get; }
}