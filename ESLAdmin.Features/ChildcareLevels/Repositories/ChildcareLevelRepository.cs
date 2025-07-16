using Dapper;
using ESLAdmin.Domain.Entities;
using ESLAdmin.Features.ChildcareLevels.CreateChildcareLevel;
using ESLAdmin.Features.ChildcareLevels.Repositories.Interfaces;
using ESLAdmin.Features.ChildcareLevels.UpdateChildcareLevel;
using ESLAdmin.Features.Exceptions;
using ESLAdmin.Features.Repositories;
using ESLAdmin.Features.Repositories.Interfaces;
using ESLAdmin.Logging.Interface;

namespace ESLAdmin.Features.ChildcareLevels.Repositories;

public static class ChildcareLevelConsts
{
  public const string PARAM_CHILDCARELEVELID = "pchildcarelevelid";
  public const string PARAM_CHILDCARELEVELNAME = "pchildcareLevelName";
  public const string PARAM_MAXCAPACITY = "pmaxcapacity";
  public const string PARAM_DISPLAYORDER = "pdisplayorder";
  public const string PARAM_INITUSER = "pinituser";
  public const string PARAM_USERCODE = "pusercode";
  public const string PARAM_GUID = "pguid";

  public const string CHILDCARELEVELID = "childcarelevelid";
  public const string CHILDCARELEVELNAME = "childcarelevelname";
  public const string MAXCAPACITY = "maxcapacity";
  public const string PLACESASSIGNED = "placesassigned";
  public const string DISPLAYORDER = "displayorder";

  public const string SQL_GETALL = 
    @"select 
        CHILDCARELEVELID as ID,
        CHILDCARELEVELNAME,
        MAXCAPACITY,
        PLACESASSIGNED,
        DISPLAYORDER,
        INITUSER,
        INITDATE,
        USERCODE,
        USERSTAMP,
        GUID      
      from
        CHILDCARELEVELS 
      order by
        CHILDCARELEVELNAME
      ";

  public const string SQL_GETBYID = 
    @"select 
        CHILDCARELEVELID as ID,
        CHILDCARELEVELNAME,
        MAXCAPACITY,
        PLACESASSIGNED,
        DISPLAYORDER,
        INITUSER,
        INITDATE,
        USERCODE,
        USERSTAMP,
        GUID      
      from
        CHILDCARELEVELS 
      where
        CHILDCARELEVELID = @id 
      order by
        CHILDCARELEVELNAME 
      ";
  public const string SP_CHILDCARELEVEL_ADD = "childcarelevel_add";
  public const string SP_CHILDCARELEVEL_UPD = "childcarelevel_upd";
  public const string SP_CHILDCARELEVEL_DEL =
    "childcarelevel_del";
}

public class ChildcareLevelRepository :
  RepositoryBase<ChildcareLevel, OperationResult>,
  IChildcareLevelRepository
{
  // =================================================
  // 
  // ChildcareLevelRepository
  //
  // ==================================================
  public ChildcareLevelRepository(
    IDbContextDapper dbContextDapper, 
    IDbContextEF? dbContextEF, 
    IMessageLogger messageLogger) 
    : base(dbContextDapper, dbContextEF, messageLogger)
  {
  }

  // =================================================
  // 
  // GetAllChildcareLevelsAsync
  //
  // ==================================================
  public async Task<IEnumerable<ChildcareLevel>> GetAllChildcareLevelsAsync()
  {
    string sql = ChildcareLevelConsts.SQL_GETALL;

    try
    {
      _messageLogger.LogSqlQuery(
        nameof(GetAllChildcareLevelsAsync),
        sql,
        null);
      return await DapQueryMultipleAsync(sql, null);
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(GetAllChildcareLevelsAsync),
        sql,
        null,
        ex);

      throw new DatabaseException(
        nameof(GetAllChildcareLevelsAsync),
        sql,
        null,
        ex);
    }
  }

  // =================================================
  // 
  // GetAllChildcareLevelByIdAsync
  //
  // ==================================================
  public async Task<ChildcareLevel?> GetChildcareLevelByIdAsync(long id)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(GetChildcareLevelByIdAsync),
        "_dbcontextDapper");

    DynamicParameters parameters = new DynamicParameters();

    string sql = ChildcareLevelConsts.SQL_GETBYID;

    parameters.AddInt64InputParam(
      OperationResultConsts.ID,
      id);

    try
    {
      _messageLogger.LogSqlQuery(
        nameof(GetChildcareLevelByIdAsync),
        sql,
        null);

      return await DapQuerySingleAsync(sql, parameters);
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(GetChildcareLevelByIdAsync),
        sql,
        null,
        ex);

      throw new DatabaseException(
        nameof(GetAllChildcareLevelsAsync),
        sql,
        null,
        ex);
    }
  }

  // =================================================
  // 
  // CreateChildcareLevelAsync
  //
  // ==================================================
  public async Task<OperationResult> CreateChildcareLevelAsync(
    CreateChildcareLevel.Mapper mapper,
    CreateChildcareLevel.Request request)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(CreateChildcareLevelAsync),
        "_dbcontextDapper");
   
    // Stored procedure
    var sql = ChildcareLevelConsts.SP_CHILDCARELEVEL_UPD;


    DynamicParameters parameters = mapper.ToEntity(request);
    var paramStr = _dbContextDapper.SerializeDynamicParameters(parameters);
    try
    {
      _messageLogger.LogDatabaseExec(
        nameof(CreateChildcareLevelAsync),
        sql,
        paramStr);

      await DapExecWithTransAsync(sql, parameters);
      return mapper.FromEntity(parameters);
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(CreateChildcareLevelAsync),
        sql,
        paramStr,
        ex);

      throw new DatabaseException(
        nameof(CreateChildcareLevelAsync),
        sql,
        paramStr,
        ex);
    }
  }

  // =================================================
  // 
  // UpdateChildcareLevelAsync
  //
  // ==================================================
  public async Task<OperationResult> UpdateChildcareLevelAsync(
    long id,
    UpdateChildcareLevel.Request request)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(UpdateChildcareLevelAsync),
        "_dbcontextDapper");

    // Stored procedure
    var sql = ChildcareLevelConsts.SP_CHILDCARELEVEL_ADD;

    DynamicParameters parameters = new DynamicParameters();
    // Input parameters
    parameters.AddInt64InputParam(
      ChildcareLevelConsts.PARAM_CHILDCARELEVELID,
      id);
    parameters.AddStringInputParam(
      ChildcareLevelConsts.PARAM_CHILDCARELEVELNAME,
      request.ChildcareLevelName,
      32);
    parameters.AddInt32InputParam(
      ChildcareLevelConsts.PARAM_MAXCAPACITY,
      request.MaxCapacity);
    parameters.AddInt32InputParam(
      ChildcareLevelConsts.PARAM_DISPLAYORDER,
      request.DisplayOrder);
    parameters.AddInt64InputParam(
      ChildcareLevelConsts.PARAM_USERCODE,
      request.UserCode);
    parameters.AddStringInputParam(
      ChildcareLevelConsts.PARAM_GUID,
      request.Guid,
      38);


    // Output parameters
    parameters.AddInt32OutputParam(
      OperationResultConsts.DBAPIERROR);
    parameters.AddStringOutputParam(
      OperationResultConsts.DUPFIELDNAME,
      128);
    parameters.AddStringOutputParam(
      OperationResultConsts.GUID,
      38);

    var paramStr = _dbContextDapper.SerializeDynamicParameters(parameters);
    try
    {
      _messageLogger.LogDatabaseExec(
        nameof(CreateChildcareLevelAsync),
        sql,
        paramStr);

      await DapExecWithTransAsync(sql, parameters);

      OperationResult operationResult =
        new OperationResult();
      operationResult.DbApiError = parameters.Get<int>(OperationResultConsts.DBAPIERROR);
      operationResult.DupFieldName = parameters.Get<string?>(OperationResultConsts.DUPFIELDNAME);
      operationResult.Guid = parameters.Get<string?>(
        OperationResultConsts.GUID);
      return operationResult;
    }
    catch (Exception ex)
    {
      _messageLogger.LogDatabaseException(
        nameof(CreateChildcareLevelAsync),
        sql,
        paramStr,
        ex);

      throw new DatabaseException(
        nameof(CreateChildcareLevelAsync),
        sql,
        paramStr,
        ex);
    }
  }

  // =================================================
  // 
  // DeleteChildcareLevelAsync
  //
  // ==================================================
  public async Task<OperationResult> DeleteChildcareLevelAsync(
    long id)
  {
    if (_dbContextDapper == null)
      throw new NullException(
        nameof(DeleteChildcareLevelAsync),
        "_dbcontextDapper");

    string sql = ChildcareLevelConsts.SP_CHILDCARELEVEL_DEL;

    DynamicParameters parameters = new DynamicParameters();
    // Input parameters
    parameters.AddInt64InputParam(
      ChildcareLevelConsts.PARAM_CHILDCARELEVELID,
      id);
    // Output parameters
    parameters.AddStringOutputParam(
      OperationResultConsts.REFERENCETABLE,
      64);
    parameters.AddInt32OutputParam(
      OperationResultConsts.DBAPIERROR);

    var paramStr = _dbContextDapper.SerializeDynamicParameters(parameters);
    try
    {
      _messageLogger.LogDatabaseExec(
        nameof(DeleteChildcareLevelAsync),
        sql,
        paramStr);

      await DapExecWithTransAsync(sql, parameters);

      OperationResult operationResult =
        new OperationResult();
      operationResult.DbApiError = parameters.Get<int>(OperationResultConsts.DBAPIERROR);
      operationResult.ReferenceTable = parameters.Get<string?>(OperationResultConsts.REFERENCETABLE);
      return operationResult;
    }
    catch (Exception ex) 
    {
      _messageLogger.LogDatabaseException(
       nameof(DeleteChildcareLevelAsync),
       sql,
       paramStr,
       ex);

      throw new DatabaseException(
        nameof(DeleteChildcareLevelAsync),
        sql,
        paramStr,
        ex);
    }
  }
}