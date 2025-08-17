using ESLAdmin.Common.Utilities;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ESLAdmin.Infrastructure.Services;

public interface ITokenBlacklistService
{
  Task AddToBlacklistAsync(
    string token, 
    string userId, 
    DateTime expiryDate, 
    CancellationToken cancellationToken = default);
  Task<bool> IsBlacklistedAsync(
    string token,
    CancellationToken cancellationToken = default);
  Task CleanupExpiredTokensAsync(
    CancellationToken cancellationToken = default);
}

public class TokenBlacklistService : ITokenBlacklistService
{
  private readonly UserDbContext _context;

  public TokenBlacklistService(UserDbContext context)
  {
    _context = context;
  }

  public async Task AddToBlacklistAsync(
    string token, 
    string userId,
    DateTime expiryDate,
    CancellationToken cancellationToken = default)
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
  }

  public async Task<bool> IsBlacklistedAsync(
    string token,
    CancellationToken cancellationToken = default)
  {
    var tokenHash = EncryptUtils.ComputeSha256Hash(token);
    return await _context.BlacklistedTokens
      .AnyAsync(bt => bt.TokenHash == tokenHash, cancellationToken);
  }

  public async Task CleanupExpiredTokensAsync(
    CancellationToken cancellationToken = default)
  {
    var expiredTokens = await _context.BlacklistedTokens
      .Where(bt => bt.ExpiryDate < DateTime.UtcNow)
      .ToListAsync(cancellationToken);

    _context.BlacklistedTokens.RemoveRange(expiredTokens);
    await _context.SaveChangesAsync(cancellationToken);
  }

  
}
