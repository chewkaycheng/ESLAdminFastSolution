using ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels;

public abstract class ChildcareLevelCommandHandlerBase<THandler>(
  IChildcareLevelRepository repository,
  ILogger<THandler> logger)
{
  protected IChildcareLevelRepository _repository { get; } = repository;
  protected ILogger<THandler> _logger { get; } = logger;
}