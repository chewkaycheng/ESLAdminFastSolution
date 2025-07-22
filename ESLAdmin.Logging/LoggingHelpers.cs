using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Logging;

public static class LoggingHelpers
{
  public static string FormatIdentityErrors(
    IEnumerable<IdentityError>? validationErrors)
  {
    var sb = new StringBuilder();
    sb.AppendLine("");
    foreach(var error in validationErrors)
    {
      sb.AppendLine($"    Code: \"{error.Code}\", Description: \"{error.Description}\"");
    }
    return sb.ToString();
  }
}
