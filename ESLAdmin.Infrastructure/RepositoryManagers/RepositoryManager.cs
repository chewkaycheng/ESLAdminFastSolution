using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Repositories;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Users.Repositories;
using ESLAdmin.Infrastructure.Data;
using ESLAdmin.Infrastructure.Data.Interfaces;
using ESLAdmin.Infrastructure.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.RepositoryManagers;

//------------------------------------------------------------------------------
//
//                       class RepositoryManager
//
//------------------------------------------------------------------------------
public class RepositoryManager : IRepositoryManager
{
  private readonly Lazy<IChildcareLevelRepository> _childcareLevelRepository;
  private readonly Lazy<IAuthenticationRepository>
    _authenticationRespository;

  //------------------------------------------------------------------------------
  //
  //                       RepositoryManager
  //
  //------------------------------------------------------------------------------
  public RepositoryManager(
    IDbContextDapper dbContextDapper,
    IMessageLogger messageLogger,
    ILogger<RepositoryManager> logger,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<IdentityRole> roleManager,
    UserDbContext dbContext)
  {
    _childcareLevelRepository = new Lazy<IChildcareLevelRepository>(
      () => new ChildcareLevelRepository(
        dbContextDapper,
        messageLogger));

    _authenticationRespository = new Lazy<IAuthenticationRepository>(
      () => new AuthenticationRepository(
        logger,
        messageLogger,
        userManager,
        signInManager,
        roleManager,
        dbContext,
        dbContextDapper));
  }

  //------------------------------------------------------------------------------
  //
  //                       ChildcareLevelRepository
  //
  //------------------------------------------------------------------------------
  public IChildcareLevelRepository ChildcareLevelRepository =>
    _childcareLevelRepository.Value;

  //------------------------------------------------------------------------------
  //
  //                       AuthenticationRepository
  //
  //------------------------------------------------------------------------------
  public IAuthenticationRepository AuthenticationRepository =>
    _authenticationRespository.Value;
}


