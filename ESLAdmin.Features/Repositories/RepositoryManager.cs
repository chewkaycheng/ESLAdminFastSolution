using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Repositories;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Features.Users.Repositories;
using ESLAdmin.Features.Users.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ESLAdmin.Features.Repositories;

public class RepositoryManager : IRepositoryManager
{
  private readonly Lazy<IChildcareLevelRepository> _childcareLevelRepository;
  private readonly Lazy<IAuthenticationRepository>
    _authenticationRespository;
  public RepositoryManager(
    IDbContextDapper dbContextDapper,
    IMessageLogger messageLogger,
    UserManager<User> userManager,
    IConfiguration configuration)
  {
    _childcareLevelRepository = new Lazy<IChildcareLevelRepository>(
      () => new ChildcareLevelRepository(
        dbContextDapper,
        messageLogger));

    _authenticationRespository = new Lazy<IAuthenticationRepository>(
      () => new AuthenticationRepository(
        messageLogger,
        userManager,
        configuration));
  }
  public IChildcareLevelRepository ChildcareLevelRepository => 
    _childcareLevelRepository.Value;

  public IAuthenticationRepository AuthenticationRepository =>
    _authenticationRespository.Value;
}


