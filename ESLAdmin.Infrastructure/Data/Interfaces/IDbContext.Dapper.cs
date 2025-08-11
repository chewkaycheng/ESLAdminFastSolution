using Dapper;
using ErrorOr;
using System.Data;

namespace ESLAdmin.Infrastructure.Data.Interfaces;

public interface IDbContextDapper
{
  IDbConnection GetConnection();

  Task<ErrorOr<IDbConnection>> GetConnectionAsync();

  Task<ErrorOr<IDbTransaction>> GetTransactionAsync(
    IDbConnection connection,
    CancellationToken cancellationToken = default);

  string SerializeDynamicParameters(DynamicParameters parameters);
}
