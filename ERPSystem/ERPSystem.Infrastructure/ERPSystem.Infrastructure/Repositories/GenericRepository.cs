using ERPSystem.Core.Entities;
using ERPSystem.Core.Interfaces;
using ERPSystem.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ERPSystem.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ErpDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ErpDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet
                .AsNoTracking() 
                .Where(x => x.Id == id && x.IsActive);

            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<T?> GetByIdForUpdateAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(x => x.Id == id && x.IsActive);
            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet
                .AsNoTracking() 
                .Where(x => x.IsActive);

            if (filter != null)
                query = query.Where(filter);

            query = ApplyIncludes(query, includes);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Where(predicate);

            query = ApplyIncludes(query, includes);

            return await query.ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive);

            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> FirstOrDefaultForUpdateAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(x => x.IsActive);

            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<(IEnumerable<T> Data, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet
                .AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            query = ApplyIncludes(query, includes);

            if (orderBy != null)
                query = orderBy(query);
            else
                query = query.OrderByDescending(x => x.CreatedDate);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive);

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .AnyAsync(predicate);
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedDate = DateTime.Now;
            entity.IsActive = true;

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            var entityList = entities.ToList();

            foreach (var entity in entityList)
            {
                entity.CreatedDate = DateTime.Now;
                entity.IsActive = true;
            }

            await _dbSet.AddRangeAsync(entityList);
            await _context.SaveChangesAsync();

            return entityList;
        }

        public async Task UpdateAsync(T entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            var entityList = entities.ToList();

            foreach (var entity in entityList)
            {
                entity.UpdatedDate = DateTime.Now;
            }

            _dbSet.UpdateRange(entityList);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdForUpdateAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        public async Task DeleteAsync(T entity)
        {
            entity.IsActive = false;
            entity.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            var entityList = entities.ToList();

            foreach (var entity in entityList)
            {
                entity.IsActive = false;
                entity.UpdatedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        // Include'ları uygula
        private static IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[]? includes)
        {
            if (includes is { Length: > 0 })
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }
    }
}