using Dapper;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Entities;
using FastEndpoints;

namespace ESLAdmin.Features.ClassLevels.Endpoints.GetClassLevel;

public class GetClassLevelMapper : Mapper<
  EmptyRequest,
  GetClassLevelResponse,
  ClassLevel>
{
  //------------------------------------------------------------------------------
  //
  //                           FromEntity 
  //
  //------------------------------------------------------------------------------
  public override GetClassLevelResponse FromEntity(
    ClassLevel classLevel) => new()
  {
    Id = classLevel.ClassLevelId,
    ClassLevelName = classLevel.ClassLevelName,
    DisplayOrder = classLevel.DisplayOrder,
    DisplayColor = classLevel.DisplayColor,
    InitUser = classLevel.InitUser,
    InitDate = classLevel.InitDate,
    UserCode = classLevel.UserCode,
    UserStamp = classLevel.UserStamp,
    Guid = classLevel.Guid
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
      DbConstsClassLevel.CLASSLEVELID,
      id);
    return parameters;
  }
}