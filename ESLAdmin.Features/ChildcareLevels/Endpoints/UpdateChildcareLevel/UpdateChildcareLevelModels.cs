using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.UpdateChildcareLevel;

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelRequest
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelRequest
{
  public long ChildcareLevelId { get; set; }
  public string ChildcareLevelName { get; set; } = string.Empty;
  public int MaxCapacity { get; set; }
  public int DisplayOrder { get; set; }
  public long UserCode { get; set; }
  public string Guid { get; set; } = string.Empty;
}

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelCommand
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelCommand : ICommand<Results<Ok<UpdateChildcareLevelResponse>,
    ProblemDetails,
    InternalServerError>>
{
  public long ChildcareLevelId { get; set; }
  public string ChildcareLevelName { get; set; } = string.Empty;
  public int MaxCapacity { get; set; }
  public int DisplayOrder { get; set; }
  public long UserCode { get; set; }
  public string Guid { get; set; } = string.Empty;
  public UpdateChildcareLevelMapper mapper { get; set; }
}

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelResponse
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelResponse
{
  public long ChildcareLevelId { get; set; }
  public string Guid { get; set; }
}

//------------------------------------------------------------------------------
//
//                           class UpdateChildcareLevelValidator
//
//------------------------------------------------------------------------------
public class UpdateChildcareLevelValidator : Validator<UpdateChildcareLevelRequest>
{
  public UpdateChildcareLevelValidator()
  {

  }
}
