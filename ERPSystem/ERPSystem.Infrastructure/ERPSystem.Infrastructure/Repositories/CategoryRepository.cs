using ERPSystem.Core.DTOs.Category;
using ERPSystem.Core.DTOs.Common;
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

namespace ERPSystem.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ErpDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetByCodeAsync(string code)
        {
            return await FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await ExistsAsync(x => x.Code == code);
        }

        public async Task<bool> ExistsByCodeAsync(string code, int excludeId)
        {
            return await ExistsAsync(x => x.Code == code && x.Id != excludeId);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await ExistsAsync(x => x.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId)
        {
            return await ExistsAsync(x => x.Name == name && x.Id != excludeId);
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            return await FindAsync(x => x.ParentCategoryId == null);
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId)
        {
            return await FindAsync(x => x.ParentCategoryId == parentId);
        }

        public async Task<IEnumerable<Category>> GetAllChildrenRecursiveAsync(int parentId)
        {
            var allCategories = await GetAllAsync();
            var children = new List<Category>();
            await GetChildrenRecursive(parentId, allCategories, children);
            return children;
        }

        private async Task GetChildrenRecursive(int parentId, IEnumerable<Category> allCategories, List<Category> result)
        {
            var directChildren = allCategories.Where(x => x.ParentCategoryId == parentId);
            foreach (var child in directChildren)
            {
                result.Add(child);
                await GetChildrenRecursive(child.Id, allCategories, result);
            }
        }

        public async Task<IEnumerable<Category>> GetCategoryPathAsync(int categoryId)
        {
            var path = new List<Category>();
            var current = await GetByIdAsync(categoryId);

            while (current != null)
            {
                path.Insert(0, current);
                if (current.ParentCategoryId.HasValue)
                {
                    current = await GetByIdAsync(current.ParentCategoryId.Value);
                }
                else
                {
                    break;
                }
            }

            return path;
        }

        public async Task<bool> IsDescendantOfAsync(int childId, int ancestorId)
        {
            var current = await GetByIdAsync(childId);

            while (current?.ParentCategoryId != null)
            {
                if (current.ParentCategoryId == ancestorId)
                    return true;

                current = await GetByIdAsync(current.ParentCategoryId.Value);
            }

            return false;
        }

        public async Task<bool> HasChildrenAsync(int categoryId)
        {
            return await ExistsAsync(x => x.ParentCategoryId == categoryId);
        }

        public async Task<bool> HasProductsAsync(int categoryId)
        {
            return await _context.Set<Product>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .AnyAsync(x => x.CategoryId == categoryId);
        }

        public async Task<int> GetProductCountAsync(int categoryId)
        {
            return await _context.Set<Product>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .CountAsync(x => x.CategoryId == categoryId);
        }

        public async Task<int> GetChildrenCountAsync(int categoryId)
        {
            return await CountAsync(x => x.ParentCategoryId == categoryId);
        }

        public async Task<int> GetLevelAsync(int categoryId)
        {
            var level = 0;
            var current = await GetByIdAsync(categoryId);

            while (current?.ParentCategoryId != null)
            {
                level++;
                current = await GetByIdAsync(current.ParentCategoryId.Value);
            }

            return level;
        }
#pragma warning disable CA1862
#pragma warning disable RCS1155

        public async Task<PagedResultDto<Category>> GetPagedCategoriesAsync(CategoryFilterDto filter)
        {
            // Filter expression
            Expression<Func<Category, bool>>? filterExpression = x =>
                (!filter.IsActive.HasValue || x.IsActive == filter.IsActive.Value) &&
                (string.IsNullOrEmpty(filter.Code) || x.Code.ToLower().Contains(filter.Code.ToLower())) &&
                (string.IsNullOrEmpty(filter.Name) || x.Name.ToLower().Contains(filter.Name.ToLower())) &&
                (!filter.ParentCategoryId.HasValue || x.ParentCategoryId == filter.ParentCategoryId.Value) &&
                (!filter.CreatedDateFrom.HasValue || x.CreatedDate >= filter.CreatedDateFrom.Value) &&
                (!filter.CreatedDateTo.HasValue || x.CreatedDate <= filter.CreatedDateTo.Value) &&
                (string.IsNullOrEmpty(filter.SearchTerm) ||
                 x.Code.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                 x.Name.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                 (x.Description != null && x.Description.ToLower().Contains(filter.SearchTerm.ToLower())));

            // Order by
            Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null;
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "code":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.Code) :
                            q => q.OrderBy(x => x.Code);
                        break;
                    case "name":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.Name) :
                            q => q.OrderBy(x => x.Name);
                        break;
                    case "createddate":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.CreatedDate) :
                            q => q.OrderBy(x => x.CreatedDate);
                        break;
                    default:
                        orderBy = q => q.OrderBy(x => x.Name);
                        break;
                }
            }
            else
            {
                orderBy = q => q.OrderBy(x => x.Name);
            }

            var (data, totalCount) = await GetPagedAsync(
                filter.PageNumber,
                filter.PageSize,
                filterExpression,
                orderBy,
                x => x.ParentCategory
            );

            return new PagedResultDto<Category>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<IEnumerable<Category>> GetCategoryTreeAsync(int? parentId = null, int? maxLevel = null)
        {
            var query = _dbSet.AsNoTracking().Where(x => x.IsActive);

            if (parentId.HasValue)
            {
                query = query.Where(x => x.ParentCategoryId == parentId.Value);
            }
            else
            {
                query = query.Where(x => x.ParentCategoryId == null);
            }

            var categories = await query
                .Include(x => x.ParentCategory)
                .OrderBy(x => x.Name)
                .ToListAsync();

            // Recursive olarak alt kategorileri yükle
            if (maxLevel == null || maxLevel > 0)
            {
                foreach (var category in categories)
                {
                    category.SubCategories = (await GetCategoryTreeAsync(category.Id, maxLevel.HasValue ? maxLevel - 1 : null)).ToList();
                }
            }

            return categories;
        }

        // Override base methods to include ParentCategory by default
        public override async Task<Category?> GetByIdAsync(int id, params Expression<Func<Category, object>>[] includes)
        {
            var allIncludes = new List<Expression<Func<Category, object>>> { x => x.ParentCategory };
            if (includes != null && includes.Length > 0)
            {
                allIncludes.AddRange(includes);
            }

            return await base.GetByIdAsync(id, allIncludes.ToArray());
        }
    }
}
