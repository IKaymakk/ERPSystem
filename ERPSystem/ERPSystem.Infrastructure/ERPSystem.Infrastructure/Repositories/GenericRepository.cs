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

        // ID ile tek kayıt getir
        public virtual async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(x => x.Id == id && x.IsActive);

            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync();
        }

        // Tüm kayıtları getir (filtrelenebilir)
        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(x => x.IsActive);

            if (filter != null)
                query = query.Where(filter);

            query = ApplyIncludes(query, includes);

            return await query.ToListAsync();
        }

        // Koşula göre kayıtları bul
        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(x => x.IsActive).Where(predicate);

            query = ApplyIncludes(query, includes);

            return await query.ToListAsync();
        }

        // İlk kaydı getir veya null döndür
        public async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(x => x.IsActive);

            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync(predicate);
        }

        // Sayfalanmış veri getir
        public async Task<(IEnumerable<T> Data, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(x => x.IsActive);

            // Filter uygula
            if (filter != null)
                query = query.Where(filter);

            // Toplam sayıyı al (include'lardan önce)
            var totalCount = await query.CountAsync();

            // Include'ları uygula
            query = ApplyIncludes(query, includes);

            // Sıralama uygula
            if (orderBy != null)
                query = orderBy(query);
            else
                query = query.OrderByDescending(x => x.CreatedDate);

            // Sayfalama uygula
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        // Kayıt sayısını getir
        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet.Where(x => x.IsActive);

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }

        // Kayıt var mı kontrol et
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(x => x.IsActive).AnyAsync(predicate);
        }

        // Yeni kayıt ekle
        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedDate = DateTime.Now;
            entity.IsActive = true;

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        // Toplu kayıt ekle
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

        // Kayıt güncelle
        public async Task UpdateAsync(T entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        // Toplu kayıt güncelle
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

        // Soft delete (ID ile)
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        // Soft delete (entity ile)
        public async Task DeleteAsync(T entity)
        {
            entity.IsActive = false;
            entity.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        // Toplu soft delete
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
