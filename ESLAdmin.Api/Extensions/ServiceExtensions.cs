using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Repositories;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Features.Users.Repository;
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
  public static void ConfigureIdentity(this IServiceCollection services)
  {
    var builder =
      services.AddIdentity<User, IdentityRole>(o =>
      {
        o.Password.RequireDigit = true;
        o.Password.RequireLowercase = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequiredLength = 10;
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
          configuration.GetConnectionString("ESLAdminConnection")));

    services.AddSingleton<IDbContextDapper, DbContextDapper>();
  }

  // =================================================
  // 
  // ConfigureRepositoryManagers
  //
  // ==================================================
  public static void ConfigureRepositoryManagers(
  this IServiceCollection services)
  {
    services.AddScoped<IChildcareLevelRepositoryManager,
      ChildcareLevelRepositoryManager>();
  }

}
