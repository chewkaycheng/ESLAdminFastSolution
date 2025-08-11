using Dapper;
using ErrorOr;
using ESLAdmin.Common.Exceptions;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Infrastructure.Data.Consts;
using ESLAdmin.Infrastructure.Data.Interfaces;
using ESLAdmin.Infrastructure.Repositories;
using ESLAdmin.Logging.Interface;
using Microsoft.Extensions.Logging;

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
  public async Task UpdateChildcareLevelAsync(
    DynamicParameters parameters)
  {
    var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_UPD;

    try
    {
      await DapExecWithTransAsync(sql, parameters);
      return;
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
  public async Task DeleteChildcareLevelAsync(
    DynamicParameters parameters)
  {
    try
    {
      var sql = DbConstsChildcareLevel.SP_CHILDCARELEVEL_DEL;

      await DapExecWithTransAsync(sql, parameters);
      return;
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(DeleteChildcareLevelAsync),
        ex);

      throw new DatabaseException(
        nameof(DeleteChildcareLevelAsync),
        ex);
    }
  }

}
