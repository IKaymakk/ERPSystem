using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Role;
using ERPSystem.Core.Entities;
using ERPSystem.Core.Interfaces;
using ERPSystem.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ErpDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _dbSet.AsNoTracking().Where(x => x.IsActive && x.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<PagedResultDto<Role>> GetPagedRolesAsync(RoleFilterDto filter)
        {
            IQueryable<Role> query = _dbSet.AsNoTracking()
                .Where(x => x.IsActive)
                .Include(x => x.Users.Where(u => u.IsActive));

            // Filtreleme
            if (!string.IsNullOrEmpty(filter.SearchTerm))
                query = query.Where(x => x.Name.Contains(filter.SearchTerm) ||
                                        (x.Description != null && x.Description.Contains(filter.SearchTerm)));

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.Description))
                query = query.Where(x => x.Description != null && x.Description.Contains(filter.Description));

            if (filter.IsActive.HasValue)
                query = query.Where(x => x.IsActive == filter.IsActive.Value);

            if (filter.CreatedDateFrom.HasValue)
                query = query.Where(x => x.CreatedDate >= filter.CreatedDateFrom.Value);

            if (filter.CreatedDateTo.HasValue)
                query = query.Where(x => x.CreatedDate <= filter.CreatedDateTo.Value.AddDays(1));

            // Toplam kayıt sayısı
            var totalCount = await query.CountAsync();

            // Sıralama
            query = filter.SortDescending
                ? query.OrderByDescending(GetSortExpression(filter.SortBy))
                : query.OrderBy(GetSortExpression(filter.SortBy));

            // Sayfalama
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResultDto<Role>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<Role?> GetRoleWithUsersAsync(int roleId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Users.Where(u => u.IsActive))
                .FirstOrDefaultAsync(x => x.Id == roleId && x.IsActive);
        }

        public async Task<List<Role>> GetAllRolesWithUserCountAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.IsActive)
                .Include(x => x.Users.Where(u => u.IsActive))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<bool> IsRoleAssignedToUsersAsync(int roleId)
        {
            return await _context.Set<User>()
                .AsNoTracking()
                .AnyAsync(x => x.IsActive && x.RoleId == roleId);
        }

        private static System.Linq.Expressions.Expression<Func<Role, object>> GetSortExpression(string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "name" => x => x.Name,
                "description" => x => x.Description ?? "",
                "createddate" => x => x.CreatedDate,
                "updateddate" => x => x.UpdatedDate ?? DateTime.MinValue,
                "isactive" => x => x.IsActive,
                _ => x => x.CreatedDate
            };
        }
    }
}
