using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Infrastructure.Data;
using ESLAdmin.Infrastructure.Data.Interfaces;
using ESLAdmin.Infrastructure.Repositories.Interfaces;
using ESLAdmin.Infrastructure.RepositoryManagers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ESLAdmin.Api.Extensions;

public static class ServiceExtensions
{
  // =================================================
  // 
  // ConfigureCors
  //
  // ==================================================
  public static void ConfigureCors(
      this IServiceCollection services) =>
      services.AddCors(options =>
      {
        options.AddPolicy(
          "CorsPolicy",
          builder =>
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
      });

  // =================================================
  // 
  // ConfigureIISIntegration
  //
  // ==================================================
  public static void ConfigureIISIntegration(
      this IServiceCollection services) =>
        services.Configure<IISOptions>(options =>
        {

        });

  // =================================================
  // 
  // ConfigureIdentity
  //
  // ==================================================
  public static void ConfigureIdentity(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    var passwordLength = configuration
      .GetValue<int>("Identity:Password:RequiredLength", 5);

    var builder =
      services.AddIdentity<User, IdentityRole>(o =>
      {
        o.Password.RequireDigit = true;
        o.Password.RequireLowercase = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequiredLength = passwordLength;
        o.User.RequireUniqueEmail = true;
      })
      .AddEntityFrameworkStores<UserDbContext>()
      .AddDefaultTokenProviders();
  }

  // =================================================
  // 
  // ConfigureFirebirdDbContexts
  //
  // ==================================================
  public static void ConfigureFirebirdDbContexts(
   this IServiceCollection services,
   IConfiguration configuration)
  {
    services.AddDbContext<UserDbContext>(opts =>
        opts.UseFirebird(
          configuration.GetConnectionString("ESLAdminConnection"), b=>b.MigrationsAssembly("ESLAdmin.Api")));

    services.AddSingleton<IDbContextDapper, DbContextDapper>();
    services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
  }

  // =================================================
  // 
  // ConfigureRepositoryManager
  //
  // ==================================================
  public static void ConfigureRepositoryManager(
    this IServiceCollection services)
  {
    services.AddScoped<IRepositoryManager, RepositoryManager>();
  }
}
