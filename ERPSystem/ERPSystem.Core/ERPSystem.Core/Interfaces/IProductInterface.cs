using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Product;
using ERPSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetByCodeAsync(string code);
        Task<Product?> GetByBarcodeAsync(string barcode);
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsByBarcodeAsync(string barcode);
        Task<bool> ExistsByCodeAsync(string code, int excludeProductId);
        Task<bool> ExistsByBarcodeAsync(string barcode, int excludeProductId);
        Task<PagedResultDto<Product>> GetPagedProductsAsync(ProductFilterDto filter);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<ProductStockInfoDto>> GetStockInfoAsync();
        Task UpdateStockAsync(int productId, decimal newStockAmount);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    }
}
