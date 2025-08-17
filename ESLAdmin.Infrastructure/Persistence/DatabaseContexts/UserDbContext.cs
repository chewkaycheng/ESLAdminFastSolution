using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ESLAdmin.Infrastructure.Persistence.DatabaseContexts;

public class UserDbContext : IdentityDbContext<User>
{
  public UserDbContext(DbContextOptions options)
    : base(options)
  {
  }

  public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

  public DbSet<RefreshToken> RefreshTokens { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<RefreshToken>()
      .HasOne(rt => rt.User)
      .WithMany()
      .HasForeignKey(rt => rt.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<BlacklistedToken>(entity =>
    {
      entity.HasKey(e => e.Id); // Primary key on Token
      entity.Property(e => e.Token)
        .IsRequired().HasMaxLength(2048); // Adjust length for JWT size
      entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(64); // SHA-256 hash
      entity.Property(e => e.UserId)
        .HasMaxLength(450); //  Match IdentityUser Id length
      entity.HasIndex(e => e.TokenHash).IsUnique(); // Index on TokenHash
      entity.HasIndex(e => e.UserId); // Index for querying by user
      entity.HasIndex(e => e.ExpiryDate); // Index for cleanup queries
    });
  }
}
