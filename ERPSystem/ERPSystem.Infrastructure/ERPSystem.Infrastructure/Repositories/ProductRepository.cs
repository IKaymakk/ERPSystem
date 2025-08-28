using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Product;
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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ErpDbContext context) : base(context)
        {

        }
        public async Task<bool> ExistsByBarcodeAsync(string barcode)
        {
            return await ExistsAsync(x => x.Barcode == barcode);
        }

        public async Task<bool> ExistsByBarcodeAsync(string barcode, int excludeProductId)
        {
            return await ExistsAsync(x => x.Barcode == barcode && x.Id == excludeProductId);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await ExistsAsync(x => x.Code == code);
        }

        public async Task<bool> ExistsByCodeAsync(string code, int excludeProductId)
        {
            return await ExistsAsync(x => x.Code == code && x.Id != excludeProductId);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await GetAllAsync(x => x.IsActive,
                x => x.Category,
                x => x.Unit);
        }

        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            return await FirstOrDefaultAsync(
                                      x => x.Barcode == barcode,
                                      x => x.Category,
                                      x => x.Unit
                                      );
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await FindAsync(x => x.CategoryId == categoryId,
                x => x.Category,
                x => x.Unit);
        }

        public async Task<Product?> GetByCodeAsync(string code)
        {
            return await FirstOrDefaultAsync(
                                      x => x.Code == code,
                                      x => x.Category,
                                      x => x.Unit
                                      );
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await FindAsync(
                x => x.CurrentStock <= x.MinStockLevel,
                x => x.Category,
                x => x.Unit);
        }

        public async Task<PagedResultDto<Product>> GetPagedProductsAsync(ProductFilterDto filter)
        {
            Expression<Func<Product, bool>>? filterExpression = x =>
                (!filter.CategoryId.HasValue || x.CategoryId == filter.CategoryId.Value) &&
                (!filter.UnitId.HasValue || x.UnitId == filter.UnitId.Value) &&
                (!filter.IsActive.HasValue || x.IsActive == filter.IsActive.Value) &&
                (!filter.IsLowStock.HasValue || (filter.IsLowStock.Value ? x.CurrentStock <= x.MinStockLevel : x.CurrentStock > x.MinStockLevel)) &&
                (!filter.MinPrice.HasValue || x.SalePrice >= filter.MinPrice.Value) &&
                (!filter.MaxPrice.HasValue || x.SalePrice <= filter.MaxPrice.Value) &&
                (!filter.CreatedDateFrom.HasValue || x.CreatedDate >= filter.CreatedDateFrom.Value) &&
                (!filter.CreatedDateTo.HasValue || x.CreatedDate <= filter.CreatedDateTo.Value) &&
                (string.IsNullOrEmpty(filter.SearchTerm) ||
                 x.Name.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                 x.Code.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                 x.Description.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                 x.Barcode.ToLower().Contains(filter.SearchTerm.ToLower())) &&
                (string.IsNullOrEmpty(filter.Barcode) || x.Barcode == filter.Barcode);

            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null;
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "name":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.Name) :
                            q => q.OrderBy(x => x.Name);
                        break;
                    case "code":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.Code) :
                            q => q.OrderBy(x => x.Code);
                        break;
                    case "saleprice":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.SalePrice) :
                            q => q.OrderBy(x => x.SalePrice);
                        break;
                    case "currentstock":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.CurrentStock) :
                            q => q.OrderBy(x => x.CurrentStock);
                        break;
                    case "createddate":
                        orderBy = filter.SortDescending ?
                            q => q.OrderByDescending(x => x.CreatedDate) :
                            q => q.OrderBy(x => x.CreatedDate);
                        break;
                    default:
                        orderBy = q => q.OrderBy(x => x.Id);
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
                x => x.Category,
                x => x.Unit
            );

            return new PagedResultDto<Product>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<IEnumerable<ProductStockInfoDto>> GetStockInfoAsync()
        {
            var products = await _dbSet
              .AsNoTracking()
              .Where(x => x.IsActive)
              .Include(x => x.Unit)
              .Select(x => new ProductStockInfoDto
              {
                  ProductId = x.Id,
                  ProductCode = x.Code,
                  ProductName = x.Name,
                  CurrentStock = x.CurrentStock,
                  MinStockLevel = x.MinStockLevel,
                  IsLowStock = x.CurrentStock <= x.MinStockLevel,
                  UnitSymbol = x.Unit.Symbol
              })
              .ToListAsync();

            return products;

        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await FindAsync(
                 x => x.Name.ToLower().Contains(searchTerm.ToLower()) ||
                      x.Code.ToLower().Contains(searchTerm.ToLower()) ||
                      x.Barcode.ToLower().Contains(searchTerm.ToLower()),
                 x => x.Category,
                 x => x.Unit
             );
        }

        public async Task UpdateStockAsync(int productId, decimal newStockAmount)
        {
            var product = await GetByIdForUpdateAsync(productId);
            if (product != null)
            {
                product.CurrentStock = newStockAmount;
                product.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
