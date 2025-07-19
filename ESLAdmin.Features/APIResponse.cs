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
}
