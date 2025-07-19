using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Models;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels.Mappers;

//------------------------------------------------------------------------------
//
//                           class ChildcareLevelMapper 
//
//------------------------------------------------------------------------------
public class ChildcareLevelMapper : Mapper<
  EmptyRequest, 
  ChildcareLevelResponse, 
  ChildcareLevel>
{
  //------------------------------------------------------------------------------
  //
  //                           FromEntity 
  //
  //------------------------------------------------------------------------------
  public override ChildcareLevelResponse FromEntity(ChildcareLevel e) => new()
  {
    Id = e.Id,
    ChildcareLevelName = e.ChildcareLevelName,
    MaxCapacity = e.MaxCapacity,
    DisplayOrder = e.DisplayOrder,
    PlacesAssigned = e.PlacesAssigned,
    InitUser = e.InitUser,
    InitDate = e.InitDate,
    UserCode = e.UserCode,
    UserStamp = e.UserStamp,
    Guid = e.Guid
  };
}
