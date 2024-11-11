using System.Linq.Expressions;
using ToDo.Domain.Common;

namespace ToDo.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<PaginatedResult<T>> GetPagedAsync(
            IQueryable<T> query,
            Expression<Func<T, object>> orderBy,
            PaginationRequest paginationRequest);
    }
} 