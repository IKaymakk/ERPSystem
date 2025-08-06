using ERPSystem.Core.Entities;
using ERPSystem.Core.Interfaces;
using ERPSystem.Infrastructure.DBContext;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Unit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ERPSystem.Infrastructure.Repositories;

public class UnitRepository : GenericRepository<Unit>, IUnitRepository
{
    public UnitRepository(ErpDbContext context) : base(context)
    {
    }

    public async Task<Unit?> GetBySymbolAsync(string symbol)
    {
        // Symbol'e göre Unit bul
        // Büyük-küçük harf duyarsız arama yap
        return await FirstOrDefaultAsync(x => x.Symbol.ToLower() == symbol.ToLower()
        );
    }

    public async Task<Unit?> GetByNameAsync(string name)
    {
        // Name'e göre Unit bul 
        // Büyük-küçük harf duyarsız arama yap
        return await FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower()
        );
    }

    public async Task<bool> ExistsBySymbolAsync(string symbol)
    {
        // Symbol bazında var mı kontrolü
        return await ExistsAsync(x => x.Symbol.ToLower() == symbol.ToLower());
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        // Name bazında var mı kontrolü  
        return await ExistsAsync(x => x.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> ExistsBySymbolAsync(string symbol, int excludeUnitId)
    {
        // Güncelleme sırasında - mevcut kaydı hariç tutarak Symbol kontrolü
        return await ExistsAsync(x =>
            x.Symbol.ToLower() == symbol.ToLower() &&
            x.Id != excludeUnitId
        );
    }

    public async Task<bool> ExistsByNameAsync(string name, int excludeUnitId)
    {
        // Güncelleme sırasında - mevcut kaydı hariç tutarak Name kontrolü
        return await ExistsAsync(x =>
            x.Name.ToLower() == name.ToLower() &&
            x.Id != excludeUnitId
        );
    }

    public async Task<PagedResultDto<Unit>> GetPagedUnitsAsync(UnitFilterDto filter)
    {
        // Dinamik filtreleme Expression'ı oluştur
        Expression<Func<Unit, bool>>? filterExpression = x =>
            // IsActive filtresi - eğer belirtilmişse
            (!filter.IsActive.HasValue || x.IsActive == filter.IsActive.Value) &&

            // Tarih aralığı filtreleri
            (!filter.CreatedDateFrom.HasValue || x.CreatedDate >= filter.CreatedDateFrom.Value) &&
            (!filter.CreatedDateTo.HasValue || x.CreatedDate <= filter.CreatedDateTo.Value) &&

            // Arama terimi - Name, Symbol veya Description'da ara
            (string.IsNullOrEmpty(filter.SearchTerm) ||
             x.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
             x.Symbol.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
             (!string.IsNullOrEmpty(x.Description) && x.Description.ToLower().Contains(filter.SearchTerm.ToLower())));

        // Sıralama mantığı - dinamik OrderBy
        Func<IQueryable<Unit>, IOrderedQueryable<Unit>>? orderBy = null;
        if (!string.IsNullOrEmpty(filter.SortBy))
            switch (filter.SortBy.ToLower())
            {
                case "name":
                    orderBy = filter.SortDescending
                        ? q => q.OrderByDescending(x => x.Name)
                        : q => q.OrderBy(x => x.Name);
                    break;
                case "symbol":
                    orderBy = filter.SortDescending
                        ? q => q.OrderByDescending(x => x.Symbol)
                        : q => q.OrderBy(x => x.Symbol);
                    break;
                case "createddate":
                    orderBy = filter.SortDescending
                        ? q => q.OrderByDescending(x => x.CreatedDate)
                        : q => q.OrderBy(x => x.CreatedDate);
                    break;
                default:
                    // Varsayılan sıralama
                    orderBy = q => q.OrderBy(x => x.Name);
                    break;
            }
        else
            // SortBy belirtilmemişse varsayılan sıralama
            orderBy = q => q.OrderBy(x => x.Name);

        // Base repository'den sayfalanmış veriyi al
        var (data, totalCount) = await GetPagedAsync(
            filter.PageNumber,
            filter.PageSize,
            filterExpression,
            orderBy
        );

        // PagedResultDto'ya dönüştür ve döndür
        return new PagedResultDto<Unit>
        {
            Items = data,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<IEnumerable<Unit>> GetActiveUnitsAsync()
    {
        // Tüm aktif Unit'leri getir - Dropdown'lar için
        // Name'e göre sıralı getir
        return await GetAllAsync(
            x => x.IsActive,
            null // Unit'in navigation property'si yok, include gereksiz
        );
    }

    public async Task<bool> IsUnitUsedInProductsAsync(int unitId)
    {
        // Bu Unit'i kullanan Product var mı kontrolü
        // Direct database sorgusu - daha performanslı
        return await _context.Set<Product>()
            .AsNoTracking()
            .Where(p => p.IsActive && p.UnitId == unitId)
            .AnyAsync();
    }

    public async Task<int> GetProductCountByUnitAsync(int unitId)
    {
        // Bu Unit'i kullanan aktif ürün sayısı
        // İstatistik ve raporlama için kullanılabilir
        return await _context.Set<Product>()
            .AsNoTracking()
            .Where(p => p.IsActive && p.UnitId == unitId)
            .CountAsync();
    }
}