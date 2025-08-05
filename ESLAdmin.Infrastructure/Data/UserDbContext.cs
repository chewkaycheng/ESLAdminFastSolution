using ESLAdmin.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ESLAdmin.Infrastructure.Data;

public class UserDbContext : IdentityDbContext<User>
{
  public UserDbContext(DbContextOptions options)
    : base(options)
  {
  }

  public DbSet<RefreshToken> RefreshTokens { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<RefreshToken>()
      .HasOne(rt => rt.User)
      .WithMany()
      .HasForeignKey(rt => rt.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
