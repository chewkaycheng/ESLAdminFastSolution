using Dapper;
using System.Data;

namespace ESLAdmin.Infrastructure.Data.Interfaces;

public interface IDbContextDapper
{
  IDbConnection GetConnection();

  Task<IDbConnection> GetConnectionAsync();

  Task<IDbTransaction> GetTransactionAsync(IDbConnection connection);

  string SerializeDynamicParameters(DynamicParameters parameters);
}
