using Dapper;
using ErrorOr;
using System.Data;

namespace ESLAdmin.Infrastructure.Persistence.DatabaseContexts.Interfaces;

public interface IDbContextDapper
{
  ErrorOr<IDbConnection> GetConnection();

  Task<ErrorOr<IDbConnection>> GetConnectionAsync();

  Task<ErrorOr<IDbTransaction>> GetTransactionAsync(
    IDbConnection connection,
    CancellationToken cancellationToken = default);

  string SerializeDynamicParameters(DynamicParameters parameters);
}
