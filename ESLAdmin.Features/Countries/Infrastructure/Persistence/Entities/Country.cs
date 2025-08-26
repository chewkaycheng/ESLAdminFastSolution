using ESLAdmin.Infrastructure.Persistence.Entities;

namespace ESLAdmin.Features.Countries.Infrastructure.Persistence.Entities;

public class Country : EntityBase
{
  public long CountryId { get; set; } = 0;
  public string CountryName { get; set; } = string.Empty;
  public string LanguageName { get; set; } = string.Empty;
}