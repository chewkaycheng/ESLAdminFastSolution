using Dapper;
using ErrorOr;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Constants;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Entities;
using ESLAdmin.Features.Common.Infrastructure.Persistence.Repositories.Interfaces;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESLAdmin.Features.Common.Infrastructure.Persistence.Repositories;

//------------------------------------------------------------------------------
//
//                        class ClassLevelRepository
//
//------------------------------------------------------------------------------
public class ClassLevelRepository :
  RepositoryBase<ClassLevel, OperationResult>,
  IClassLevelRepository
{
  //------------------------------------------------------------------------------
  //
  //                        ClassLevelRepository
  //
  //------------------------------------------------------------------------------
  public ClassLevelRepository(
    IDbContextDapper dbContextDapper,
    ILogger<ClassLevelRepository> logger) :
    base(dbContextDapper, logger)
  {
  }
  
  //------------------------------------------------------------------------------
  //
  //                        GetClassLevelsAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<IEnumerable<ClassLevel>>> GetClassLevelsAsync()
  {
    var sql = DbConstsClassLevel.SQL_GETALL;

    var classLevelsResult = 
      await DapQueryMultipleAsync(sql, null);

    if (classLevelsResult.IsError)
    {
      return classLevelsResult.Errors;
    }
    var classLevels = classLevelsResult.Value;
    return ErrorOrFactory.From(classLevels);
  }

  //------------------------------------------------------------------------------
  //
  //                        GetClassLevelAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<ClassLevel?>> GetClassLevelAsync( 
    DynamicParameters parameters)
  {
    var sql = DbConstsClassLevel.SQL_GETBYID;
    var classLevelResult = await DapQuerySingleAsync(
      sql,
      parameters);
    if (classLevelResult.IsError)
    {
      return classLevelResult.Errors;
    }

    return classLevelResult.Value;
  }

  //------------------------------------------------------------------------------
  //
  //                        CreateClassLevelAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> CreateClassLevelAsync(DynamicParameters parameters)
  {
    var sql = DbConstsClassLevel.SP_CLASSLEVEL_ADD;

    var result = await DapExecWithTransAsync(sql, parameters);
    if (result.IsError)
    {
      return result.Errors;
    }

    return new Success();
  }

  //------------------------------------------------------------------------------
  //
  //                        UpdateClassLevelAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> UpdateClassLevelAsync(DynamicParameters parameters)
  {
    var sql = DbConstsClassLevel.SP_CLASSLEVEL_UPD;

    var result = await DapExecWithTransAsync(sql, parameters);
    if (result.IsError)
    {
      return result.Errors;
    }
    return new Success();
  }

  //------------------------------------------------------------------------------
  //
  //                        DeleteClassLevelAsync
  //
  //------------------------------------------------------------------------------
  public async Task<ErrorOr<Success>> DeleteClassLevelAsync(DynamicParameters parameters)
  {
    var sql = DbConstsClassLevel.SP_CLASSLEVEL_DEL;

    var result = await DapExecWithTransAsync(sql, parameters);
    if (result.IsError)
    {
      return result.Errors;
    }

    return new Success();
  }
}