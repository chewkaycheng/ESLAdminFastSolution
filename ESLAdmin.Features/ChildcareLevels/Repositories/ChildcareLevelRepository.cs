using Dapper;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;

namespace ESLAdmin.Features.ChildcareLevels.Repositories;

public class ChildcareLevelRepository : 
  RepositoryBase<ChildcareLevel, OperationResult>, 
  IChildcareLevelRepository
{
  public ChildcareLevelRepository(
    IDbContextDapper dbContextDapper,
    IMessageLogger messageLogger)
    : base(dbContextDapper, messageLogger)
  {
  }

  public async Task<APIResponse<OperationResult>> CreateChildcareLevel(
    CreateChildcareLevel.Request request, 
    CreateChildcareLevel.Mapper mapper)
  {
    try
    {
      DynamicParameters parameters = mapper.ToEntity(request);
      var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_ADD;

      await DapExecWithTransAsync(sql, parameters);
      var result = mapper.FromEntity(parameters);

      if (result.DbApiError == 0)
      {
        return new APIResponse<OperationResult>
        {
          IsSuccess = true,
          Data = result
        };
      }
      else
      {
        return new APIResponse<OperationResult>
        {
          IsSuccess = false,
          Error = $"Another record with the childcare level name: {request.ChildcareLevelName} already exists.",
          Data = result
        };
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(CreateChildcareLevel),
        ex);

      throw new DatabaseException(
        nameof(CreateChildcareLevel),
        ex);
    }
  }

  public async Task<APIResponse<OperationResult>> DeleteChildcareLevel(
    long id,
    DeleteChildcareLevel.Mapper mapper)
  {
    try
    {
      var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_DEL;

      DynamicParameters parameters = mapper.ToEntity(id);

      await DapExecWithTransAsync(sql, parameters);
      OperationResult result = mapper.FromEntity(parameters);

      if (result.DbApiError == 0)
      {
        return new APIResponse<OperationResult>
        {
          IsSuccess = true,
          Data = result
        };
      }
      else
      {
        return new APIResponse<OperationResult>
        {
          IsSuccess = false,
          Error = $"Cannot delete Childcare level. It is being used by {result.ReferenceTable}."
        };
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(GetChildcareLevel),
        ex);

      throw new DatabaseException(
        nameof(GetChildcareLevel),
        ex);
    }
  }

  public async Task<APIResponse<GetChildcareLevel.Response>> GetChildcareLevel(
    long id, 
    GetChildcareLevel.Mapper mapper)
  {
    try
    {
      var sql = DbConstsChildcareLevel.SQL_GETBYID;

      DynamicParameters parameters = new DynamicParameters();
      parameters.AddInt64InputParam(
        OperationResultConsts.ID,
        id);

      ChildcareLevel? childcareLevel = await DapQuerySingleAsync(
        sql,
        parameters);
      if (childcareLevel == null)
      {
        var apiResponse = new APIResponse<GetChildcareLevel.Response>();
        apiResponse.IsSuccess = false;
        apiResponse.Error = $"A childcare level with Id: {id} does not exist.";
        return apiResponse;
      }
      else
      {
        GetChildcareLevel.Response childcareLevelResponse =
          mapper.FromEntity(childcareLevel);
        var apiResponse = new APIResponse<GetChildcareLevel.Response>();
        apiResponse.IsSuccess = true;
        apiResponse.Data = childcareLevelResponse;
        return apiResponse;
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(GetChildcareLevel), 
        ex);

      throw new DatabaseException(
        nameof(GetChildcareLevel),
        ex);
    }
  }

  public async Task<APIResponse<IEnumerable<GetChildcareLevels.Response>>> GetChildcareLevels(GetChildcareLevels.Mapper mapper)
  {
    try
    {
      var sql = DbConstsChildcareLevel.SQL_GETALL;

      var childcareLevels = await DapQueryMultipleAsync(sql, null);
      IEnumerable<GetChildcareLevels.Response> childcareLevelsResponse = 
        childcareLevels.Select(
          childcareLevel => mapper.FromEntity(
            childcareLevel)).ToList();

      var response = new APIResponse<IEnumerable<GetChildcareLevels.Response>>();
      response.IsSuccess = true;
      response.Data = childcareLevelsResponse;

      return response;
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(GetChildcareLevels), 
        ex);

      throw new DatabaseException(
        nameof(GetChildcareLevels),
        ex);
    }
  }

  public async Task<APIResponse<OperationResult>> UpdateChildcareLevel(
    UpdateChildcareLevel.Request request, 
    UpdateChildcareLevel.Mapper mapper)
  {
    DynamicParameters parameters = mapper.ToEntity(request);
    var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_UPD;

    try
    {
      await DapExecWithTransAsync(sql, parameters);
      var result = mapper.FromEntity(parameters);

      if (result.DbApiError == 0)
      {
        return new APIResponse<OperationResult>
        {
          IsSuccess = true,
          Data = result
        };
      }
      else
      {
        return new APIResponse<OperationResult>
        {
          IsSuccess = false,
          Error = $"Another record with the childcare level name: {request.ChildcareLevelName} already exists."
        };
      }
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(UpdateChildcareLevel),
        ex);

      throw new DatabaseException(
        nameof(UpdateChildcareLevel),
        ex);
    }
  }
}
