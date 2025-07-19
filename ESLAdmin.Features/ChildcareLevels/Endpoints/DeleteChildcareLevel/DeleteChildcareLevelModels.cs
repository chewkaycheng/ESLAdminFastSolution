using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.Endpoints.DeleteChildcareLevel;

//------------------------------------------------------------------------------
//
//                        class DeleteChildcareLevelRequest
//
//------------------------------------------------------------------------------
public class DeleteChildcareLevelRequest
{
  public long Id { get; set; }
}

//------------------------------------------------------------------------------
//
//                        DeleteChildcareLevelValidator
//
//------------------------------------------------------------------------------
public class DeleteChildcareLevelValidator : Validator<DeleteChildcareLevelRequest>
{
  public DeleteChildcareLevelValidator()
  { 
  }
}