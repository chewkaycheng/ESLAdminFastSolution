using Dapper;
using ESLAdmin.Features.ClassLevels.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using FastEndpoints;

namespace ESLAdmin.Features.ClassLevels.Endpoints.CreateClassLevel;

//------------------------------------------------------------------------------
//
//                        Class CreateClassLevelMapper
//
//------------------------------------------------------------------------------
public class CreateClassLevelMapper : Mapper<
  CreateClassLevelCommand,
  OperationResult,
  DynamicParameters>
{
  //------------------------------------------------------------------------------
  //
  //                           ToParameters
  //
  //------------------------------------------------------------------------------
  public DynamicParameters ToParameters(CreateClassLevelCommand command)
  {
    DynamicParameters parameters = new DynamicParameters();
    parameters.AddStringInputParam(
      DbConstsClassLevel.PARAM_CLASSLEVELNAME,
      command.ClassLevelName,
      32);
    parameters.AddInt32InputParam(
      DbConstsClassLevel.PARAM_DISPLAYORDER,
      command.DisplayOrder);
    parameters.AddInt32InputParam(
      DbConstsClassLevel.PARAM_DISPLAYCOLOR,
      command.DisplayColor);
    parameters.AddInt64InputParam(
      DbConstsClassLevel.PARAM_INITUSER,
      command.InitUser);

    // Output parameters
    parameters.AddInt32OutputParam(
      DbConstsOperationResult.DBAPIERROR);
    parameters.AddStringOutputParam(
      DbConstsOperationResult.DUPFIELDNAME,
      128);
    parameters.AddInt64OutputParam(
      DbConstsClassLevel.CLASSLEVELID);
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
  public OperationResult FromParameters(
    DynamicParameters parameters) => new()
    {
      DbApiError = parameters.Get<int>(
      DbConstsOperationResult.DBAPIERROR),
      DupFieldName = parameters.Get<string?>(
      DbConstsOperationResult.DUPFIELDNAME),
      Id = parameters.Get<long?>(
      DbConstsClassLevel.CLASSLEVELID),
      Guid = parameters.Get<string?>(
      DbConstsOperationResult.GUID),
    };

  //------------------------------------------------------------------------------
  //
  //                           ToResponse
  //
  //------------------------------------------------------------------------------
  public CreateClassLevelResponse ToResponse(DynamicParameters parameters)
  {
    return new CreateClassLevelResponse
    {
      ClassLevelId = parameters.Get<long>(
        DbConstsClassLevel.CLASSLEVELID),
      Guid = parameters.Get<string>(
        DbConstsOperationResult.GUID)
    };
  }
}
