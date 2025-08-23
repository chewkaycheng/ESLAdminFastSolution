using Dapper;
using ESLAdmin.Infrastructure.Persistence;
using ESLAdmin.Infrastructure.Persistence.Constants;
using FastEndpoints;

namespace ESLAdmin.Features.ChildcareLevels;

//------------------------------------------------------------------------------
//
//                        Class CreateChildcareLevelMapper
//
//------------------------------------------------------------------------------
public class CreateChildcareLevelMapper : Mapper<
  CreateChildcareLevelCommand,
  OperationResult,
  DynamicParameters>
{
  //------------------------------------------------------------------------------
  //
  //                           ToParameters
  //
  //------------------------------------------------------------------------------
  public DynamicParameters ToParameters(CreateChildcareLevelCommand command)
  {
    DynamicParameters parameters = new DynamicParameters();
    parameters.AddStringInputParam(
      DbConstsChildcareLevel.PARAM_CHILDCARELEVELNAME,
      command.ChildcareLevelName,
      32);
    parameters.AddInt32InputParam(
      DbConstsChildcareLevel.PARAM_MAXCAPACITY,
      command.MaxCapacity);
    parameters.AddInt32InputParam(
      DbConstsChildcareLevel.PARAM_DISPLAYORDER,
      command.DisplayOrder);
    parameters.AddInt64InputParam(
      DbConstsChildcareLevel.PARAM_INITUSER,
      command.InitUser);

    // Output parameters
    parameters.AddInt32OutputParam(
      DbConstsOperationResult.DBAPIERROR);
    parameters.AddStringOutputParam(
      DbConstsOperationResult.DUPFIELDNAME,
      128);
    parameters.AddInt64OutputParam(
      DbConstsChildcareLevel.CHILDCARELEVELID);
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
      DbConstsChildcareLevel.CHILDCARELEVELID),
      Guid = parameters.Get<string?>(
      DbConstsOperationResult.GUID),
    };

  //------------------------------------------------------------------------------
  //
  //                           ToResponse
  //
  //------------------------------------------------------------------------------
  public CreateChildcareLevelResponse ToResponse(DynamicParameters parameters)
  {
    return new CreateChildcareLevelResponse
    {
      ChildcareLevelId = parameters.Get<long>(
        DbConstsChildcareLevel.CHILDCARELEVELID),
      Guid = parameters.Get<string>(
        DbConstsOperationResult.GUID)
    };

  }
}
