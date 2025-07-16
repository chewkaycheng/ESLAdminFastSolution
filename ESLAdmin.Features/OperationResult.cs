namespace ESLAdmin.Features;

public static class OperationResultConsts
{
  public const string DBAPIERROR = "dbapierror";
  public const string DUPFIELDNAME = "dupfieldname";
  public const string REFERENCETABLE = "referencetable";
  public const string ID = "id";
  public const string GUID = "guid";
}

public class OperationResult
{
  // =================================================
  // 
  // OperationResult
  //
  // ==================================================
  public OperationResult()
  {
  }

  // =================================================
  // 
  // OperationResult
  //
  // ==================================================
  public OperationResult(OperationResult operationResult)
  {
    DbApiError = operationResult.DbApiError;
    DupFieldName = operationResult.DupFieldName;
    ReferenceTable = operationResult.ReferenceTable;
    Id = operationResult.Id;
    Guid = operationResult.Guid;
  }
  public int DbApiError { get; set; }
  public string? DupFieldName { get; set; }
  public string? ReferenceTable { get; set; }
  public long? Id { get; set; }
  public string? Guid { get; set; }
}
