using Dapper;
using ESLAdmin.Common.Exceptions;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Infrastructure.Data.Consts;
using ESLAdmin.Infrastructure.Data.Interfaces;
using ESLAdmin.Infrastructure.Repositories;
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
  //public async Task<IEnumerable<ChildcareLevelResponse>> GetChildcareLevels(ChildcareLevelMapper mapper)
  //{
  //  try
  //  {
  //    var sql = DbConstsChildcareLevel.SQL_GETALL;

  //    var childcareLevels = await DapQueryMultipleAsync(sql, null);
  //    IEnumerable<ChildcareLevelResponse> childcareLevelsResponse =
  //      childcareLevels.Select(
  //        childcareLevel => mapper.FromEntity(
  //          childcareLevel)).ToList();

  //    return childcareLevelsResponse;
  //  }
  //  catch (Exception ex)
  //  {
  //    _messageLogger.LogDatabaseException(
  //      nameof(GetChildcareLevels),
  //      ex);

  //    throw new DatabaseException(
  //      nameof(GetChildcareLevels),
  //      ex);
  //  }
  //}

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevel
  //
  //------------------------------------------------------------------------------
  //public async Task<ChildcareLevelResponse> GetChildcareLevel(
  //  long id,
  //  ChildcareLevelMapper mapper)
  //{
  //  try
  //  {
  //    var sql = DbConstsChildcareLevel.SQL_GETBYID;

  //    DynamicParameters parameters = new DynamicParameters();
  //    parameters.AddInt64InputParam(
  //      OperationResultConsts.ID,
  //      id);

  //    ChildcareLevel? childcareLevel = await DapQuerySingleAsync(
  //      sql,
  //      parameters);
  //    if (childcareLevel == null)
  //    {
  //      return null;
  //    }
  //    else
  //    {
  //      ChildcareLevelResponse childcareLevelResponse =
  //        mapper.FromEntity(childcareLevel);
  //      return childcareLevelResponse;
  //    }
  //  }
  //  catch (Exception ex)
  //  {
  //    _messageLogger.LogDatabaseException(
  //      nameof(GetChildcareLevel), 
  //      ex);

  //    throw new DatabaseException(
  //      nameof(GetChildcareLevel),
  //      ex);
  //  }
  //}

  //------------------------------------------------------------------------------
  //
  //                        CreateChildcareLevel
  //
  //------------------------------------------------------------------------------
  public async Task CreateChildcareLevelAsync(
    DynamicParameters parameters)
  {
    try
    {
      var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_ADD;

      await DapExecWithTransAsync(sql, parameters);
      return;
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
  //public async Task<OperationResult> UpdateChildcareLevelAsync(
  //  UpdateChildcareLevelCommand command)
  //{
  //  DynamicParameters parameters = command.mapper.ToEntity(command);
  //  var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_UPD;

  //  try
  //  {
  //    await DapExecWithTransAsync(sql, parameters);
  //    OperationResult operationResult = command.mapper.FromEntity(parameters);
  //    return operationResult;
  //  }
  //  catch (Exception ex)
  //  {
  //    _messageLogger.LogDatabaseException(
  //      nameof(UpdateChildcareLevelAsync),
  //      ex);

  //    throw new DatabaseException(
  //      nameof(UpdateChildcareLevelAsync),
  //      ex);
  //  }
  //}

  //------------------------------------------------------------------------------
  //
  //                        DeleteChildcareLevel
  //
  //------------------------------------------------------------------------------
  //public async Task<OperationResult> DeleteChildcareLevel(
  //  long id,
  //  Endpoints.DeleteChildcareLevel.DeleteChildcareLevelMapper mapper)
  //{
  //  try
  //  {
  //    var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_DEL;

  //    DynamicParameters parameters = mapper.ToEntity(id);

  //    await DapExecWithTransAsync(sql, parameters);
  //    OperationResult operationResult = mapper.FromEntity(parameters);
  //    return operationResult;
  //  }
  //  catch (Exception ex)
  //  {
  //    _messageLogger.LogDatabaseException(
  //      nameof(GetChildcareLevel),
  //      ex);

  //    throw new DatabaseException(
  //      nameof(GetChildcareLevel),
  //      ex);
  //  }
  //}

}
