using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Features.Exceptions
{
  public class APIErrors
  {
    public List<ValidationFailure> Errors { get; set; }
  }
}
