using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Unit;
using ERPSystem.Core.Entities;
using ERPSystem.Core.Interfaces;
using global::ERPSystem.Core.DTOs.Common;
using global::ERPSystem.Core.Entities;

namespace ERPSystem.Core.Interfaces
{
    public interface IUnitRepository : IGenericRepository<Unit>
    {
        // Unit'e özel iş mantığı metodları

        /// <summary>
        /// Sembol (Symbol) ile Unit arama - Benzersiz olması gerekiyor
        /// Örnek: "kg", "lt", "adet" gibi
        /// </summary>
        Task<Unit?> GetBySymbolAsync(string symbol);

        /// <summary>
        /// İsim ile Unit arama - Benzersiz olması gerekiyor
        /// Örnek: "Kilogram", "Litre", "Adet" gibi
        /// </summary>
        Task<Unit?> GetByNameAsync(string name);

        /// <summary>
        /// Sembol bazında benzersizlik kontrolü (Ekleme için)
        /// </summary>
        Task<bool> ExistsBySymbolAsync(string symbol);

        /// <summary>
        /// İsim bazında benzersizlik kontrolü (Ekleme için)
        /// </summary>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>
        /// Güncelleme sırasında benzersizlik kontrolü - mevcut kaydı hariç tut
        /// </summary>
        Task<bool> ExistsBySymbolAsync(string symbol, int excludeUnitId);
        Task<bool> ExistsByNameAsync(string name, int excludeUnitId);

        /// <summary>
        /// Filtrelenmiş ve sayfalanmış Unit listesi
        /// Filter: Name, Symbol, IsActive gibi kriterlere göre arama
        /// </summary>
        Task<PagedResultDto<Unit>> GetPagedUnitsAsync(UnitFilterDto filter);

        /// <summary>
        /// Aktif tüm Unit'leri getir - Dropdown/Select box'lar için
        /// </summary>
        Task<IEnumerable<Unit>> GetActiveUnitsAsync();

        /// <summary>
        /// Bu Unit'i kullanan ürün var mı kontrolü - Silme öncesi
        /// Eğer ürünlerde kullanılıyorsa silinmemeli
        /// </summary>
        Task<bool> IsUnitUsedInProductsAsync(int unitId);

        /// <summary>
        /// Bu Unit'i kullanan ürün sayısını getir - İstatistik için
        /// </summary>
        Task<int> GetProductCountByUnitAsync(int unitId);
    }
}

