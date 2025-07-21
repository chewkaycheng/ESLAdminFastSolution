using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Repositories;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Features.Users.Repositories;
using ESLAdmin.Features.Users.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Repositories;

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
        roleManager,
        dbContext));
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


