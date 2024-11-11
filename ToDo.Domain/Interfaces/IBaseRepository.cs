using System.Linq.Expressions;
using ToDo.Domain.Common;

namespace ToDo.Domain.Interfaces;

public interface IBaseRepository<TEntity, TId> where TEntity : class
{
    // Basic CRUD
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TId id);
    
    // Optional filtering and pagination
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<PaginatedResult<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        PaginationRequest? paginationRequest = null);
    
    // Existence check
    Task<bool> ExistsAsync(TId id);
} 