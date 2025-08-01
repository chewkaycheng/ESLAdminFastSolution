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
