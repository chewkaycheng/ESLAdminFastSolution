using FluentValidation.Results;

namespace ESLAdmin.Features;

//------------------------------------------------------------------------------
//
//                       class APIResponse
//
//------------------------------------------------------------------------------
public class APIResponse<T>
{
  public bool IsSuccess { get; set; }
  public string? Error { get; set; }
  public T? Data { get; set; }
  public List<ValidationFailure> Errors { get; set; }

  public APIResponse()
  {
    Errors = new List<ValidationFailure>(); 
  }
}
