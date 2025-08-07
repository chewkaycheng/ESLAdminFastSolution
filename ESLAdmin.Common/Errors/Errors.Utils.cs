using FluentValidation.Results;

namespace ESLAdmin.Common.Errors;

public static class ErrorUtils
{
  public static List<ValidationFailure> CreateFailureList(
    string propertyName,
    string errorMessage)
  {
    return new List<ValidationFailure>
    {
      new ValidationFailure
      {
        PropertyName = propertyName,
        ErrorMessage = errorMessage
      }
    };
  }
}
