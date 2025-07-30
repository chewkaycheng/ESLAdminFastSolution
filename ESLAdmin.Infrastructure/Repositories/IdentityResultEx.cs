using Microsoft.AspNetCore.Identity;

namespace ESLAdmin.Infrastructure.Repositories;

//------------------------------------------------------------------------------
//
//                       enum IdentityErrors
//
//------------------------------------------------------------------------------
public enum IdentityErrorTypes
{
  None = 0,
  DeleteUserError,
  RemoveFromRolesError,
}

//------------------------------------------------------------------------------
//
//                       class IdentityResultEx
//
//------------------------------------------------------------------------------
public class IdentityResultEx
{
  public IdentityResult InnerResult { get; init; }

  public bool Succeeded => InnerResult.Succeeded;
  public IEnumerable<IdentityError> Errors => InnerResult.Errors;
  public string Id { get; init; }
  public IdentityErrorTypes ErrorType { get; set; }

  //------------------------------------------------------------------------------
  //
  //                       class IdentityResultEx
  //
  //-------------------------------------------------------------------------------
  // Constructor for success with ID
  public IdentityResultEx(string id)
  {
    Id = id;
    InnerResult = IdentityResult.Success;
  }

  //------------------------------------------------------------------------------
  //
  //                       class IdentityResultEx
  //
  //-------------------------------------------------------------------------------
  // Constructor for failure
  public IdentityResultEx(IEnumerable < IdentityError> errors)
  {
    Id = string.Empty;
    InnerResult = IdentityResult.Failed(errors.ToArray());
  }

  //------------------------------------------------------------------------------
  //
  //                       class IdentityResultEx
  //
  //-------------------------------------------------------------------------------
  // Constructor for failure with identity error type
  public IdentityResultEx(IdentityErrorTypes errorType, IEnumerable<IdentityError> errors)
  {
    Id = string.Empty;
    ErrorType = errorType;
    InnerResult = IdentityResult.Failed(errors.ToArray());
  }

  //------------------------------------------------------------------------------
  //
  //                              Success
  //
  //-------------------------------------------------------------------------------
  // Static success factory method
  public static IdentityResultEx Success(string id) => 
  new IdentityResultEx(id);

  //------------------------------------------------------------------------------
  //
  //                              Failed
  //
  //-------------------------------------------------------------------------------
  // Static failure factory method
  public static IdentityResultEx Failed(params IdentityError[] errors) => 
    new IdentityResultEx(errors);

  //------------------------------------------------------------------------------
  //
  //                              Failed
  //
  //-------------------------------------------------------------------------------
  // Static failure factory method
  public static IdentityResultEx Failed(IdentityErrorTypes errorType, params IdentityError[] errors) =>
    new IdentityResultEx(errorType, errors);

}
