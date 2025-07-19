namespace ESLAdmin.Features.Exceptions;

//------------------------------------------------------------------------------
//
//                       class NullOrEmptyException
//
//------------------------------------------------------------------------------
public class NullOrEmptyException : Exception
{
  //------------------------------------------------------------------------------
  //
  //                       NullOrEmptyException
  //
  //------------------------------------------------------------------------------
  public NullOrEmptyException(
    string nullObjectName,
    string funcName
    )
    : base(string.Format(
        "{0} cannot be null or blank. \nFunction: {1}",
        nullObjectName, 
        funcName))
  {    
  }
}
