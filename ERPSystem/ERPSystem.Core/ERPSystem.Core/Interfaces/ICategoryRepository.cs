using ERPSystem.Core.DTOs.Category;
using ERPSystem.Core.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERPSystem.Core.Entities;

namespace ERPSystem.Core.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetByCodeAsync(string code);
        Task<Category?> GetByNameAsync(string name);
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(string code, int excludeId);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int excludeId);
        Task<IEnumerable<Category>> GetRootCategoriesAsync();
        Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId);
        Task<IEnumerable<Category>> GetAllChildrenRecursiveAsync(int parentId);
        Task<IEnumerable<Category>> GetCategoryPathAsync(int categoryId);
        Task<bool> IsDescendantOfAsync(int childId, int ancestorId);
        Task<bool> HasChildrenAsync(int categoryId);
        Task<bool> HasProductsAsync(int categoryId);
        Task<int> GetProductCountAsync(int categoryId);
        Task<int> GetChildrenCountAsync(int categoryId);
        Task<int> GetLevelAsync(int categoryId);
        Task<PagedResultDto<Category>> GetPagedCategoriesAsync(CategoryFilterDto filter);
        Task<IEnumerable<Category>> GetCategoryTreeAsync(int? parentId = null, int? maxLevel = null);
    }
}
