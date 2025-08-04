using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Role;
using ERPSystem.Core.Entities;
using ERPSystem.Core.Interfaces;
using ERPSystem.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ERPSystem.Core.Exceptions;

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
            // Filter expression
            Expression<Func<Role, bool>>? filterExpression = x =>
                x.IsActive &&
                (!filter.IsActive.HasValue || x.IsActive == filter.IsActive.Value) &&
                (!filter.CreatedDateFrom.HasValue || x.CreatedDate >= filter.CreatedDateFrom.Value) &&
                (!filter.CreatedDateTo.HasValue || x.CreatedDate <= filter.CreatedDateTo.Value.AddDays(1)) &&
                (string.IsNullOrEmpty(filter.SearchTerm) ||
                 x.Name.Contains(filter.SearchTerm) ||
                 (x.Description != null && x.Description.Contains(filter.SearchTerm))) &&
                (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                (string.IsNullOrEmpty(filter.Description) ||
                 (x.Description != null && x.Description.Contains(filter.Description)));

            // Order by
            Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null;
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "name":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.Name) :
                            q => q.OrderBy(x => x.Name);
                        break;
                    case "description":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.Description) :
                            q => q.OrderBy(x => x.Description);
                        break;
                    case "createddate":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.CreatedDate) :
                            q => q.OrderBy(x => x.CreatedDate);
                        break;
                    case "isactive":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.IsActive) :
                            q => q.OrderBy(x => x.IsActive);
                        break;
                    default:
                        orderBy = q => q.OrderBy(x => x.Id);
                        break;
                }
            }
            else
            {
                orderBy = q => q.OrderBy(x => x.Id);
            }

            // Base repository'den veriyi al
            var (data, totalCount) = await GetPagedAsync(
                filter.PageNumber,
                filter.PageSize,
                filterExpression,
                orderBy,
                x => x.Users.Where(u => u.IsActive) // Include expression
            );

            // PagedResultDto'ya dönüştür
            return new PagedResultDto<Role>
            {
                Items = data,
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

    }
}
