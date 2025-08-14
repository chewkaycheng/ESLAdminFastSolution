using System.Security.Cryptography;
using System.Text;

namespace ESLAdmin.Infrastructure.Utilities;

public static class EncryptUtils
{
  public static string ComputeSha256Hash(string input)
  {
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(input);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToHexString(hash).ToLowerInvariant();
  }
}
