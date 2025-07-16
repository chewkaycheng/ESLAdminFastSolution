using Dapper;
using System.Data;

namespace ESLAdmin.Features; 

public static class DynamicParametersExtension
{
  public static void AddDateTimeOffsetInputParam(
    this DynamicParameters parameters,
    string paramName,
    object paramValue
    )
  {
    parameters.Add(
      name: paramName,
      value: paramValue,
      dbType: DbType.DateTimeOffset,
      direction: ParameterDirection.Input
      );
  }
  public static void AddDateTimeOffsetOutputParam(
    this DynamicParameters parameters,
    string paramName
    )
  {
    parameters.Add(
      name: paramName,
      dbType: DbType.DateTimeOffset,
      direction: ParameterDirection.Output
      );
  }
  public static void AddDateTimeInputParam(
    this DynamicParameters parameters,
    string paramName,
    object paramValue
    )
  {
    parameters.Add(
      name: paramName,
      value: paramValue,
      dbType: DbType.DateTime,
      direction: ParameterDirection.Input
      );
  }
  public static void AddDateTimeOutputParam(
    this DynamicParameters parameters,
    string paramName
    )
  {
    parameters.Add(
      name: paramName,
      dbType: DbType.DateTime,
      direction: ParameterDirection.Output
      );
  }
  public static void AddDoubleInputParam(
    this DynamicParameters parameters,
    string paramName,
    object paramValue
    )
  {
    parameters.Add(
      name: paramName,
      value: paramValue,
      dbType: DbType.Double,
      direction: ParameterDirection.Input
      );
  }
  public static void AddDoubleOutputParam(
    this DynamicParameters parameters,
    string paramName
    )
  {
    parameters.Add(
      name: paramName,
      dbType: DbType.Double,
      direction: ParameterDirection.Output
      );
  }
  public static void AddSingleInputParam(
    this DynamicParameters parameters,
    string paramName,
    object paramValue
    )
  {
    parameters.Add(
      name: paramName,
      value: paramValue,
      dbType: DbType.Single,
      direction: ParameterDirection.Input
      );
  }
  public static void AddSingleOutputParam(
    this DynamicParameters parameters,
    string paramName
    )
  {
    parameters.Add(
      name: paramName,
      dbType: DbType.Single,
      direction: ParameterDirection.Output
      );
  }
  public static void AddInt32InputParam(
    this DynamicParameters parameters,
    string paramName,
    object paramValue
    )
  {
    parameters.Add(
      name: paramName,
      value: paramValue,
      dbType: DbType.Int32,
      direction: ParameterDirection.Input
      );
  }
  public static void AddInt32OutputParam(
    this DynamicParameters parameters,
    string paramName
    )
  {
    parameters.Add(
      name: paramName,
      dbType: DbType.Int32,
      direction: ParameterDirection.Output
      );
  }
  public static void AddInt64InputParam(
    this DynamicParameters parameters,
    string paramName,
    object paramValue
    )
  {
    parameters.Add(
      name: paramName,
      value: paramValue,
      dbType: DbType.Int64,
      direction: ParameterDirection.Input
      );
  }
  public static void AddInt64OutputParam(
    this DynamicParameters parameters,
    string paramName
    )
  {
    parameters.Add(
      name: paramName,
      dbType: DbType.Int64,
      direction: ParameterDirection.Output
      );
  }
  public static void AddStringInputParam(
        this DynamicParameters parameters,
        string paramName,
        object paramValue,
        int size)
  {
    parameters.Add(
      name: paramName,
      value: paramValue,
      dbType: DbType.String,
      size: size,
      direction: ParameterDirection.Input);
  }
  public static void AddStringOutputParam(
    this DynamicParameters parameters,
    string paramName,      
    int size)
  {
    parameters.Add(
      name: paramName,
      dbType: DbType.String,
      size: size,
      direction: ParameterDirection.Output);
  }
}
