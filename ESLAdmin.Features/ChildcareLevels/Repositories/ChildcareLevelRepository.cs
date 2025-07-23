using Dapper;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Endpoints.CreateChildcareLevel;
using ESLAdmin.Features.ChildcareLevels.Endpoints.UpdateChildcareLevel;
using ESLAdmin.Features.ChildcareLevels.Mappers;
using ESLAdmin.Features.ChildcareLevels.Models;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;

namespace ESLAdmin.Features.ChildcareLevels.Repositories;

//------------------------------------------------------------------------------
//
//                        class ChildcareLevelRepository
//
//------------------------------------------------------------------------------
public class ChildcareLevelRepository : 
  RepositoryBase<ChildcareLevel, OperationResult>, 
  IChildcareLevelRepository
{
  //------------------------------------------------------------------------------
  //
  //                        ChildcareLevelRepository
  //
  //------------------------------------------------------------------------------
  public ChildcareLevelRepository(
    IDbContextDapper dbContextDapper,
    IMessageLogger messageLogger)
    : base(dbContextDapper, messageLogger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevels
  //
  //------------------------------------------------------------------------------
  public async Task<IEnumerable<ChildcareLevelResponse>> GetChildcareLevels(ChildcareLevelMapper mapper)
  {
    try
    {
      var sql = DbConstsChildcareLevel.SQL_GETALL;

      var childcareLevels = await DapQueryMultipleAsync(sql, null);
      IEnumerable<ChildcareLevelResponse> childcareLevelsResponse =
        childcareLevels.Select(
          childcareLevel => mapper.FromEntity(
            childcareLevel)).ToList();

      return childcareLevelsResponse;
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

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevel
  //
  //------------------------------------------------------------------------------
  public async Task<ChildcareLevelResponse> GetChildcareLevel(
    long id,
    ChildcareLevelMapper mapper)
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
        return null;
      }
      else
      {
        ChildcareLevelResponse childcareLevelResponse =
          mapper.FromEntity(childcareLevel);
        return childcareLevelResponse;
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

  //------------------------------------------------------------------------------
  //
  //                        CreateChildcareLevel
  //
  //------------------------------------------------------------------------------
  public async Task<OperationResult> CreateChildcareLevelAsync(
    CreateChildcareLevelRequest request,
    CreateChildcareLevelMapper mapper)
  {
    try
    {
      DynamicParameters parameters = mapper.ToEntity(request);
      var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_ADD;

      await DapExecWithTransAsync(sql, parameters);
      OperationResult operationResult = mapper.FromEntity(parameters);
      return operationResult;
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(CreateChildcareLevelAsync),
        ex);

      throw new DatabaseException(
        nameof(CreateChildcareLevelAsync),
        ex);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                        UpdateChildcareLevel
  //
  //------------------------------------------------------------------------------
  public async Task<OperationResult> UpdateChildcareLevelAsync(
    UpdateChildcareLevelCommand command)
  {
    DynamicParameters parameters = command.mapper.ToEntity(command);
    var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_UPD;

    try
    {
      await DapExecWithTransAsync(sql, parameters);
      OperationResult operationResult = command.mapper.FromEntity(parameters);
      return operationResult;

      //if (result.DbApiError == 100)
      //{
      //  return new APIResponse<OperationResult>
      //  {
      //    IsSuccess = false,
      //    Error = $"Another record with the childcare level name: {request.ChildcareLevelName} already exists."
      //  };
      //}
      //else if (result.DbApiError == 200)
      //{
      //  return new APIResponse<OperationResult>
      //  {
      //    IsSuccess = false,
      //    Error = $"The record has been altered."
      //  };
      //}
      //else if (result.DbApiError == 300)
      //{
      //  return new APIResponse<OperationResult>
      //  {
      //    IsSuccess = false,
      //    Error = $"The record has been deleted."
      //  };
      //}
      //else if (result.DbApiError == 500)
      //{
      //  return new APIResponse<OperationResult>
      //  {
      //    IsSuccess = false,
      //    Error = $"The maximum capacity has been reached."
      //  };
      //}
      //else
      //{
      //  return new APIResponse<OperationResult>
      //  {
      //    IsSuccess = true,
      //    Data = result
      //  };
      //}
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(UpdateChildcareLevelAsync),
        ex);

      throw new DatabaseException(
        nameof(UpdateChildcareLevelAsync),
        ex);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                        DeleteChildcareLevel
  //
  //------------------------------------------------------------------------------
  public async Task<OperationResult> DeleteChildcareLevel(
    long id,
    Endpoints.DeleteChildcareLevel.DeleteChildcareLevelMapper mapper)
  {
    try
    {
      var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_DEL;

      DynamicParameters parameters = mapper.ToEntity(id);

      await DapExecWithTransAsync(sql, parameters);
      OperationResult operationResult = mapper.FromEntity(parameters);
      return operationResult;
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

}
