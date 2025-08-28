using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Product;
using ERPSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces
{
    public interface IProductService:IGenericService<Product>
    {
        // Advanced Query Operations
        Task<PagedResultDto<ProductDto>> GetPagedAsync(ProductFilterDto filter);
        Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
        Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm);

        // Stock Management Operations
        Task<IEnumerable<ProductStockInfoDto>> GetLowStockProductsAsync();
        Task<IEnumerable<ProductStockInfoDto>> GetStockInfoAsync();
        Task UpdateStockAsync(int productId, decimal newStockAmount);

        // Business Logic Operations
        Task<string> GenerateBarcodeAsync();
        Task<string> GenerateProductCodeAsync();
        Task<bool> IsCodeUniqueAsync(string code, int? excludeProductId = null);
        Task<bool> IsBarcodeUniqueAsync(string barcode, int? excludeProductId = null);
        Task<decimal> CalculateProfitMarginAsync(decimal salePrice, decimal purchasePrice);
    }
}
