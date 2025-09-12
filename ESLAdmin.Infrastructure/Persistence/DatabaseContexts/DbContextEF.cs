using ESLAdmin.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESLAdmin.Infrastructure.Persistence.DatabaseContexts;

public class DbContextEF : DbContext
{
  public DbContextEF(DbContextOptions options)
    : base(options)
  {
  }

  public DbSet<ClassLevel> ClassLevels { get; set; }
}
