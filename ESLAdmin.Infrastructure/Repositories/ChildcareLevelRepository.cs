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
  //                        GetChildcareLevelsAsync
  //
  //------------------------------------------------------------------------------
  public async Task<IEnumerable<ChildcareLevel>> GetChildcareLevelsAsync()
  {
    try
    {
      var sql = DbConstsChildcareLevel.SQL_GETALL;

      var childcareLevels = await DapQueryMultipleAsync(sql, null);

      return childcareLevels;
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(GetChildcareLevelsAsync),
        ex);

      throw new DatabaseException(
        nameof(GetChildcareLevelsAsync),
        ex);
    }
  }

  //------------------------------------------------------------------------------
  //
  //                        GetChildcareLevelAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ChildcareLevel?> GetChildcareLevelAsync(
   DynamicParameters parameters)
  {
    try
    {
      var sql = DbConstsChildcareLevel.SQL_GETBYID;

      ChildcareLevel? childcareLevel = await DapQuerySingleAsync(
        sql,
        parameters);
      return childcareLevel;
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(GetChildcareLevelAsync),
        ex);

      throw new DatabaseException(
        nameof(GetChildcareLevelAsync),
        ex);
    }
  }

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
