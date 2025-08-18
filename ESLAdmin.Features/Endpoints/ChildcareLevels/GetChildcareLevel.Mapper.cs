using Dapper;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using FastEndpoints;

namespace ESLAdmin.Features.Endpoints.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                           class ChildcareLevelMapper 
//
//------------------------------------------------------------------------------
public class GetChildcareLevelMapper : Mapper<
  EmptyRequest,
  GetChildcareLevelResponse,
  ChildcareLevel>
{
  //------------------------------------------------------------------------------
  //
  //                           FromEntity 
  //
  //------------------------------------------------------------------------------
  public override GetChildcareLevelResponse FromEntity(ChildcareLevel childcareLevel) => new()
  {
    Id = childcareLevel.Id,
    ChildcareLevelName = childcareLevel.ChildcareLevelName,
    MaxCapacity = childcareLevel.MaxCapacity,
    DisplayOrder = childcareLevel.DisplayOrder,
    PlacesAssigned = childcareLevel.PlacesAssigned,
    InitUser = childcareLevel.InitUser,
    InitDate = childcareLevel.InitDate,
    UserCode = childcareLevel.UserCode,
    UserStamp = childcareLevel.UserStamp,
    Guid = childcareLevel.Guid
  };

  //------------------------------------------------------------------------------
  //
  //                           ToParameter 
  //
  //------------------------------------------------------------------------------
  public DynamicParameters ToParameters(long id)
  {
    DynamicParameters parameters = new DynamicParameters();
    parameters.AddInt64InputParam(
      DbConstsChildcareLevel.PARAM_CHILDCARELEVELID,
      id);
    return parameters;
  }
}
