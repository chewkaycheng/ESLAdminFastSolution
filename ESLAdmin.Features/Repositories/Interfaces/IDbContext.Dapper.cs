using Dapper;
using System.Data;

namespace ESLAdmin.Features.Repositories.Interfaces;

public interface IDbContextDapper
{
  IDbConnection GetConnection();

  Task<IDbConnection> GetConnectionAsync();

  Task<IDbTransaction> GetTransactionAsync(IDbConnection connection);

  string SerializeDynamicParameters(DynamicParameters parameters);
}
