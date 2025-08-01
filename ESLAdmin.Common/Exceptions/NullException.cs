namespace ESLAdmin.Common.Exceptions;

//------------------------------------------------------------------------------
//
//                       class NullException
//
//------------------------------------------------------------------------------
public class NullException : Exception
{
  //------------------------------------------------------------------------------
  //
  //                       NullException
  //
  //------------------------------------------------------------------------------
  public NullException(
    string funcName,
    string nullObjectName)
    : base(
      string.Format("{0} cannot be null. \nFunction: {1}",
        nullObjectName,
        funcName))
  {
  }
}
