using ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevel;
using ESLAdmin.Infrastructure.Persistence.Entities;

namespace ESLAdminClient.Services;
public class ClassLevelService
{
  private readonly HttpClient _http;

  public ClassLevelService(HttpClient http)
  {
    _http = http;
  }

  public async Task<IQueryable<GetClassLevelResponse>> GetClassLevelAsync()
  {
    var response = await _http.GetAsync($"api/classlevels");
    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetClassLevelResponse>>();
    return result.AsQueryable();
  }

  public class PagedResult<T>
  {
    public IEnumerable<T> Data { get; set; }
  }
}
