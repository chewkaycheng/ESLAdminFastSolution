using Dapper;
using ErrorOr;
using ESLAdmin.Features.ClassLevels.Infrastructure.Persistence.Entities;

namespace ESLAdmin.Features.ClassLevels.Infrastructure.Persistence.Repositories;

public interface IClassLevelRepository
{
  Task<ErrorOr<IEnumerable<ClassLevel>>> GetClassLevelsAsync();
  
  Task<ErrorOr<ClassLevel?>> GetClassLevelAsync(
    DynamicParameters parameters);

  Task<ErrorOr<Success>> CreateClassLevelAsync(
    DynamicParameters parameters);
  
  Task<ErrorOr<Success>> UpdateClassLevelAsync(
    DynamicParameters parameters);
  
  Task<ErrorOr<Success>> DeleteClassLevelAsync(
    DynamicParameters parameters);
}