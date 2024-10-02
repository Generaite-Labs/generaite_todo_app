using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Common;

namespace ToDo.Infrastructure.Repositories
{
  public interface IBaseRepository<T> where T : class
  {
    Task<PaginatedResult<T>> GetPagedAsync(
        IQueryable<T> query,
        Expression<Func<T, object>> orderBy,
        PaginationRequest paginationRequest);
  }

  public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
  {
    protected readonly DbContext _context;

    protected BaseRepository(DbContext context)
    {
      _context = context;
    }

    public async Task<PaginatedResult<T>> GetPagedAsync(
        IQueryable<T> query,
        Expression<Func<T, object>> orderBy,
        PaginationRequest paginationRequest)
    {
      query = query.OrderBy(orderBy);

      if (!string.IsNullOrEmpty(paginationRequest.Cursor))
      {
        var decodedCursor = Convert.FromBase64String(paginationRequest.Cursor);
        var cursorValue = System.Text.Encoding.UTF8.GetString(decodedCursor);
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = GetPropertyFromExpression(orderBy);
        var propertyAccess = Expression.Property(parameter, property);
        var constant = Expression.Constant(Convert.ChangeType(cursorValue, property.PropertyType));
        var comparison = Expression.GreaterThan(propertyAccess, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);

        query = query.Where(lambda);
      }

      var items = await query.Take(paginationRequest.Limit + 1).ToListAsync();

      string? nextCursor = null;
      if (items.Count > paginationRequest.Limit)
      {
        var lastItem = items[paginationRequest.Limit - 1];
        var cursorProperty = GetPropertyFromExpression(orderBy);
        var cursorValue = cursorProperty.GetValue(lastItem)?.ToString();
        nextCursor = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cursorValue ?? string.Empty));
        items.RemoveAt(paginationRequest.Limit);
      }

      return new PaginatedResult<T>(items, nextCursor);
    }

    private static PropertyInfo GetPropertyFromExpression(Expression<Func<T, object>> expression)
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