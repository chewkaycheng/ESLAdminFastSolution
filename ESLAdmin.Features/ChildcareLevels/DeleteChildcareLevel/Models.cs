using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.DeleteChildcareLevel;

public class Request
{
  public long Id { get; set; }
}

public class Validator : Validator<Request>
{
  public Validator()
  {

  }
}