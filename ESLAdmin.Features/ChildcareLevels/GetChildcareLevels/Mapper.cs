using ESLAdmin.Domain.Entities;
using FastEndpoints;
using System;

namespace ESLAdmin.Features.ChildcareLevels.GetChildcareLevels;

public class Mapper : Mapper<EmptyRequest, Response, ChildcareLevel>
{
  public override Response FromEntity(ChildcareLevel e) => new()
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