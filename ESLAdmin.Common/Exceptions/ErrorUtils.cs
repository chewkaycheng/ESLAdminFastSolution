namespace ESLAdmin.Common.Exceptions
{
  public static class ErrorUtils
  {
    public static int MapHttpReturnCode(int DbApiError)
    {
      switch (DbApiError)
      {

        case 100:
          return 409;
        case 200:
          return 409;
        case 300:
          return 404;
        case 500:
          return 422;
        default:
          return 200;

      }

    }
  }
}
