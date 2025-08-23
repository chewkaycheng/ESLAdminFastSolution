using Dapper;
using ErrorOr;
using ESLAdmin.Features.ChildcareLevels.Entities;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Infrastructure.Persistence.Constants;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using ESLAdmin.Infrastructure.Persistence.Entities;
using ESLAdmin.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Repositories;

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
    ILogger logger,
    IMessageLogger messageLogger)
    : base(dbContextDapper, logger, messageLogger)
  {
  }

  //------------------------------------------------------------------------------
  //
  //                        CreateChildcareLevel
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<bool>> CreateChildcareLevelAsync(
    DynamicParameters parameters)
  {
    var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_ADD;

    var result = await DapExecWithTransAsync(sql, parameters);
    if (result.IsError)
    {
      return result.Errors;
    }
    return true;
  }

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevelsAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<IEnumerable<ChildcareLevel>>> GetChildcareLevelsAsync()
  {
    var sql = DbConstsChildcareLevel.SQL_GETALL;

    var childcareLevelsResult = await DapQueryMultipleAsync(sql, null);

    if (childcareLevelsResult.IsError)
    {
      return childcareLevelsResult.Errors;
    }
    var childcareLevels = childcareLevelsResult.Value;
    return ErrorOrFactory.From(childcareLevels);
  }

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevelAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<ChildcareLevel?>> GetChildcareLevelAsync(
   DynamicParameters parameters)
  {
    var sql = DbConstsChildcareLevel.SQL_GETBYID;
    var childcareLevelResult = await DapQuerySingleAsync(
      sql,
      parameters);
    if (childcareLevelResult.IsError)
    {
      return childcareLevelResult.Errors;
    }

    return childcareLevelResult.Value;
  }

  //------------------------------------------------------------------------------
  //
  //                        UpdateChildcareLevel
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<bool>> UpdateChildcareLevelAsync(
    DynamicParameters parameters)
  {
    var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_UPD;

    var result = await DapExecWithTransAsync(sql, parameters);
    if (result.IsError)
    {
      return result.Errors;
    }
    return true;
  }

  //------------------------------------------------------------------------------
  //
  //                        DeleteChildcareLevel
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<bool>> DeleteChildcareLevelAsync(
    DynamicParameters parameters)
  {
    var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_DEL;

    var result = await DapExecWithTransAsync(sql, parameters);
    if (result.IsError)
    {
      return result.Errors;
    }
    return true;
  }
}
