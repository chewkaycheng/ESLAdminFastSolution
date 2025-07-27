using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Common.Exceptions;

//------------------------------------------------------------------------------
//
//                       class ValidationException
//
//------------------------------------------------------------------------------
public class ValidationException : Exception
{
  public ValidationException(
    string message)
    : base(message)
  {
  }
}
