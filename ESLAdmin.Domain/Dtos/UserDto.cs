namespace ESLAdmin.Domain.Dtos
{
  public class UserDto 
  {
    public required string Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public required IList<string> Roles { get; init; }
  }
}
