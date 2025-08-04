using ERPSystem.Core.DTOs.Category;
using ERPSystem.Core.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces
{
    public interface ICategoryService
    {
        // CRUD Operations
        Task<CategoryDto> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto createDto);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateDto);
        Task DeleteAsync(int id);
        Task<PagedResultDto<CategoryDto>> GetPagedAsync(CategoryFilterDto filter);

        // Business Operations
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetChildCategoriesAsync(int parentId);
        Task<IEnumerable<CategoryTreeDto>> GetCategoryTreeAsync(int? parentId = null, int? maxLevel = null);
        Task<IEnumerable<CategoryDto>> GetCategoryPathAsync(int categoryId);
        Task<string> GetCategoryFullPathAsync(int categoryId);

        // Validation Operations
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(string code, int excludeId);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int excludeId);
        Task<bool> IsDescendantOfAsync(int childId, int ancestorId);
        Task<bool> HasChildrenAsync(int categoryId);
        Task<bool> HasProductsAsync(int categoryId);
        Task<bool> CanDeleteAsync(int categoryId);

        // Statistics
        Task<int> GetProductCountAsync(int categoryId);
        Task<int> GetChildrenCountAsync(int categoryId);
        Task<int> GetLevelAsync(int categoryId);
    }
}
