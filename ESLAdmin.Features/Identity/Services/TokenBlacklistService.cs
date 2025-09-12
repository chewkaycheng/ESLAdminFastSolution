using ErrorOr;
using ESLAdmin.Common.Utilities;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Identity.Infrastructure.Persistence.Contexts;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Identity.Services;

public interface ITokenBlacklistService
{
  Task<ErrorOr<Success>> AddToBlacklistAsync(
    string token, 
    string userId, 
    DateTime expiryDate, 
    CancellationToken cancellationToken = default);
  Task<ErrorOr<bool>> IsBlacklistedAsync(
    string token,
    CancellationToken cancellationToken = default);
  Task<ErrorOr<Success>> CleanupExpiredTokensAsync(
    CancellationToken cancellationToken = default);
}

public class TokenBlacklistService : ITokenBlacklistService
{
  private readonly UserDbContext _context;
  private readonly ILogger<TokenBlacklistService> _logger;

  public TokenBlacklistService(
    UserDbContext context,
    ILogger<TokenBlacklistService> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<ErrorOr<Success>> AddToBlacklistAsync(
    string token, 
    string userId,
    DateTime expiryDate,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var tokenHash = EncryptUtils.ComputeSha256Hash(token);
      var blacklistedToken = new BlacklistedToken
      {
        Token = token,
        TokenHash = tokenHash,
        UserId = userId,
        ExpiryDate = expiryDate,
        BlacklistedOn = DateTime.UtcNow
      };

      _context.BlacklistedTokens.Add(blacklistedToken);
      await _context.SaveChangesAsync(cancellationToken);
      return new Success();
    }
    catch (Exception ex) 
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
           .HandleException(ex, _logger);
    }
  }

  public async Task<ErrorOr<bool>> IsBlacklistedAsync(
    string token,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var tokenHash = EncryptUtils.ComputeSha256Hash(token);
      return await _context.BlacklistedTokens
        .AnyAsync(bt => bt.TokenHash == tokenHash, cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
           .HandleException(ex, _logger);
    }
  }

  public async Task<ErrorOr<Success>> CleanupExpiredTokensAsync(
    CancellationToken cancellationToken = default)
  {
    try
    {
      var expiredTokens = await _context.BlacklistedTokens
        .Where(bt => bt.ExpiryDate < DateTime.UtcNow)
        .ToListAsync(cancellationToken);

      _context.BlacklistedTokens.RemoveRange(expiredTokens);
      await _context.SaveChangesAsync(cancellationToken);
      return new Success();
    }
    catch (Exception ex)
    {
      _logger.LogException(ex);
      return DatabaseExceptionHandler
           .HandleException(ex, _logger);
    }
  }
}
