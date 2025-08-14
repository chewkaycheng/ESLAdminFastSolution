using System.ComponentModel.DataAnnotations;

namespace ESLAdmin.Domain.Entities;

public class BlacklistedToken
{
  public string Id { get; set; } = Guid.NewGuid().ToString(); // Unique identifier for the entity
  public string Token { get; set; } = string.Empty; // JWT token as primary key
  [Required, MaxLength(64)] // SHA-256 hash in hex (64 chars)
  public string TokenHash { get; set; } = string.Empty; // Indexed has
  public DateTime ExpiryDate { get; set; } // Token's original expiration for cleanup
  public string UserId { get; set; } = string.Empty; // Optional: Link to user for auditing
  public DateTime BlacklistedOn { get; set; } = DateTime.UtcNow; // When it was blacklisted
}
