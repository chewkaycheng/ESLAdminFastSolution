using FluentValidation.Results;

namespace ESLAdmin.Common.CustomErrors;

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