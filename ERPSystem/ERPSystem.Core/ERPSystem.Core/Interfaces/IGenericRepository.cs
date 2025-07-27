using ERPSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    // Pagination
    Task<(IEnumerable<T> Data, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[] includes);

    // Add/Update/Delete
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);
    Task DeleteAsync(int id);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(IEnumerable<T> entities);
}