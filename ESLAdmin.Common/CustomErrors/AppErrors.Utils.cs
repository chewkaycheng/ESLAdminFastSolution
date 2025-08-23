using FluentValidation.Results;
using ErrorOr;

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

  public static List<ValidationFailure> CreateFailureList(
    Dictionary<string, object> metadata)
  {
    var list = new List<ValidationFailure>();
    foreach (var key in metadata.Keys) {
      var value = metadata[key];
      if (value != null) {
        list.Add(new ValidationFailure
        {
          PropertyName = key,
          ErrorMessage = value.ToString()
        });
      }
    }
    return list;
  }

  public static List<ValidationFailure> CreateFailureList(
    List<Error> errors)
  {
    return errors.Select(e => new ValidationFailure(
      e.Code, e.Description)).ToList();
  }
}