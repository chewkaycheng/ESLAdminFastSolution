using ESLAdmin.Infrastructure.Persistence.Entities;

namespace ESLAdmin.Features.Common.Infrastructure.Persistence.Entities;

public class Person : EntityBase
{
  public long PersonId { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Email {  get; set; } = string.Empty;
  public string HomePhone {  get; set; } = string.Empty;
  public string HandPhone {  get; set; } = string.Empty;
  public long CountryId { get; set; }
}
