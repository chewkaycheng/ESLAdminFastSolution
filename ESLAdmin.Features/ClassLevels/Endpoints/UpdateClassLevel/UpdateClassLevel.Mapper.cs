using Dapper;
using ESLAdmin.Features.ClassLevels.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using FastEndpoints;

namespace ESLAdmin.Features.ClassLevels.Endpoints.UpdateClassLevel;

//------------------------------------------------------------------------------
//
//                           ToEntity
//
//------------------------------------------------------------------------------
public class UpdateClassLevelMapper : Mapper<
  UpdateClassLevelCommand,
  OperationResult,
  DynamicParameters>
{
  public DynamicParameters ToParameters(UpdateClassLevelCommand command)
  {
    DynamicParameters parameters = new DynamicParameters();

    parameters.AddInt64InputParam(
      DbConstsClassLevel.PARAM_CLASSLEVELID,
      command.ClassLevelId);
    parameters.AddStringInputParam(
      DbConstsClassLevel.PARAM_CLASSLEVELNAME,
      command.ClassLevelName,
      32);
    parameters.AddStringInputParam(
      DbConstsClassLevel.PARAM_DISPLAYCOLOR,
      command.ClassLevelName,
      32);
    parameters.AddInt32InputParam(
      DbConstsClassLevel.PARAM_DISPLAYORDER,
      command.DisplayOrder);
    parameters.AddInt32InputParam(
      DbConstsClassLevel.PARAM_DISPLAYCOLOR,
      command.DisplayColor);
    parameters.AddInt64InputParam(
      DbConstsClassLevel.PARAM_USERCODE,
      command.UserCode);
    parameters.AddStringInputParam(
      DbConstsClassLevel.PARAM_GUID,
      command.Guid,
      38);

    // Output parameters
    parameters.AddInt32OutputParam(
      DbConstsOperationResult.DBAPIERROR);
    parameters.AddStringOutputParam(
      DbConstsOperationResult.DUPFIELDNAME,
      128);
    parameters.AddStringOutputParam(
      DbConstsOperationResult.GUID,
      38);

    return parameters;
  }

  //------------------------------------------------------------------------------
  //
  //                           FromParameters
  //
  //------------------------------------------------------------------------------
  public OperationResult FromParameters(DynamicParameters parameters) => new()
  {
    DbApiError = parameters.Get<int>(
      DbConstsOperationResult.DBAPIERROR),
    DupFieldName = parameters.Get<string?>(
      DbConstsOperationResult.DUPFIELDNAME),
    Guid = parameters.Get<string?>(
      DbConstsOperationResult.GUID),
  };
}
