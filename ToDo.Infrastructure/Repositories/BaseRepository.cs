using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Common;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Repositories
{
    public abstract class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> 
        where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        protected virtual IQueryable<TEntity> BaseQuery => _dbSet;

        public virtual async Task<TEntity?> GetByIdAsync(TId id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            var entry = await _dbSet.AddAsync(entity);
            return entry.Entity;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(TId id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(TId id)
        {
            return await GetByIdAsync(id) != null;
        }

        public virtual async Task<PaginatedResult<TEntity>> GetPagedAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            PaginationRequest? paginationRequest = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            if (paginationRequest == null)
            {
                var allItems = await query.ToListAsync();
                return new PaginatedResult<TEntity>(allItems, null);
            }

            if (!string.IsNullOrEmpty(paginationRequest.Cursor))
            {
                var decodedCursor = Convert.FromBase64String(paginationRequest.Cursor);
                var cursorValue = System.Text.Encoding.UTF8.GetString(decodedCursor);
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var property = GetPropertyFromExpression(orderBy!);
                var propertyAccess = Expression.Property(parameter, property);
                
                object typedCursorValue;
                if (property.PropertyType == typeof(Guid))
                {
                    typedCursorValue = Guid.Parse(cursorValue);
                }
                else
                {
                    typedCursorValue = Convert.ChangeType(cursorValue, property.PropertyType);
                }
                
                var constant = Expression.Constant(typedCursorValue);
                var comparison = Expression.GreaterThan(propertyAccess, constant);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);

                query = query.Where(lambda);
            }

            var items = await query.Take(paginationRequest.Limit + 1).ToListAsync();

            string? nextCursor = null;
            var hasNextPage = items.Count > paginationRequest.Limit;
            
            if (hasNextPage)
            {
                var lastItem = items[paginationRequest.Limit - 1];
                var cursorProperty = GetPropertyFromExpression(orderBy!);
                var cursorValue = cursorProperty.GetValue(lastItem)?.ToString();
                if (cursorValue != null)
                {
                    nextCursor = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cursorValue));
                    items.RemoveAt(paginationRequest.Limit);
                }
            }

            return new PaginatedResult<TEntity>(items, nextCursor);
        }

        private static PropertyInfo GetPropertyFromExpression(Expression<Func<TEntity, object>> expression)
        {
            if (expression.Body is UnaryExpression unaryExpression)
            {
                if (unaryExpression.Operand is MemberExpression memberExpression)
                {
                    return (PropertyInfo)memberExpression.Member;
                }
            }
            else if (expression.Body is MemberExpression memberExpression)
            {
                return (PropertyInfo)memberExpression.Member;
            }

            throw new ArgumentException("The expression is not a property access expression.", nameof(expression));
        }
    }
}