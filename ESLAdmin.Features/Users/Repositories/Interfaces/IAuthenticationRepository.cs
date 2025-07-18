using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Features.Users.Repositories.Interfaces;

public interface IAuthenticationRepository
{
  Task<APIResponse<IdentityResultExtended>> RegisterUser(
    RegisterUser.Request request,
    RegisterUser.Mapper mapper);
}
