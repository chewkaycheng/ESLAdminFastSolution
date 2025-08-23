using ESLAdmin.Infrastructure.Persistence.DatabaseContexts;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using ESLAdmin.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence.Repositories;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Infrastructure.Persistence.RepositoryManagers;

//------------------------------------------------------------------------------
//
//                       class RepositoryManager
//
//------------------------------------------------------------------------------
public class RepositoryManager : IRepositoryManager
{
  //private readonly Lazy<IChildcareLevelRepository> _childcareLevelRepository;
  private readonly Lazy<IIdentityRepository>
    _identityRespository;

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
    //_childcareLevelRepository = new Lazy<IChildcareLevelRepository>(
    //  () => new ChildcareLevelRepository(
    //    dbContextDapper,
    //    logger,
    //    messageLogger));

    _identityRespository = new Lazy<IIdentityRepository>(
      () => new IdentityRepository(
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
  //public IChildcareLevelRepository ChildcareLevelRepository =>
  //  _childcareLevelRepository.Value;

  //------------------------------------------------------------------------------
  //
  //                       AuthenticationRepository
  //
  //------------------------------------------------------------------------------
  public IIdentityRepository IdentityRepository =>
    _identityRespository.Value;
}


