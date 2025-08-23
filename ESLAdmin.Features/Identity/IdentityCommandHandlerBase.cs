using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.IdentityUsers.Common;

public abstract class IdentityCommandHandlerBase<THandler>
{
  protected IdentityCommandHandlerBase(
    IIdentityRepository identityRepository,
    ILogger<THandler> logger)
  {
    IdentityRepository = identityRepository;
    Logger = logger;
  }

  protected IIdentityRepository IdentityRepository { get; }
  protected ILogger<THandler> Logger { get; }
}