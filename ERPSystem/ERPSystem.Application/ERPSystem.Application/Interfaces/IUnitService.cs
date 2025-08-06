using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Unit;

namespace ERPSystem.Application.Interfaces
{
    public interface IUnitService
    {
        // CRUD Operations

        /// <summary>
        /// ID'ye göre Unit getir
        /// NotFoundException fırlatır eğer bulunamazsa
        /// </summary>
        Task<UnitDto> GetByIdAsync(int id);

        /// <summary>
        /// Tüm aktif Unit'leri getir - Dropdown için
        /// </summary>
        Task<IEnumerable<UnitSelectDto>> GetActiveUnitsForSelectAsync();

        /// <summary>
        /// Filtrelenmiş ve sayfalanmış Unit listesi
        /// ValidationException fırlatır filter geçersizse
        /// </summary>
        Task<PagedResultDto<UnitDto>> GetPagedUnitsAsync(UnitFilterDto filter);

        /// <summary>
        /// Yeni Unit oluştur
        /// ValidationException fırlatır validation hatası varsa
        /// BusinessException fırlatır iş kuralı ihlali varsa
        /// </summary>
        Task<UnitDto> CreateAsync(CreateUnitDto createDto);

        /// <summary>
        /// Mevcut Unit'i güncelle
        /// NotFoundException fırlatır Unit bulunamazsa
        /// ValidationException fırlatır validation hatası varsa
        /// BusinessException fırlatır iş kuralı ihlali varsa
        /// </summary>
        Task<UnitDto> UpdateAsync(UpdateUnitDto updateDto);

        /// <summary>
        /// Unit'i sil (soft delete)
        /// NotFoundException fırlatır Unit bulunamazsa 
        /// BusinessException fırlatır Unit başka kayıtlarda kullanılıyorsa
        /// </summary>
        Task DeleteAsync(int id);

        // Business Logic Methods

        /// <summary>
        /// Unit'in kullanılabilirlik durumunu kontrol et
        /// Silme işlemi öncesi kontrol için kullanılır
        /// </summary>
        Task<UnitUsageInfoDto> GetUnitUsageInfoAsync(int id);

        /// <summary>
        /// Symbol'e göre Unit var mı kontrolü
        /// </summary>
        Task<bool> ExistsBySymbolAsync(string symbol);

        /// <summary>
        /// Name'e göre Unit var mı kontrolü
        /// </summary>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>
        /// Toplu Unit oluşturma - Excel import vs. için
        /// </summary>
        Task<BulkOperationResultDto<UnitDto>> BulkCreateAsync(IEnumerable<CreateUnitDto> createDtos);

        /// <summary>
        /// Unit aktif/pasif durumunu değiştir
        /// </summary>
        Task<UnitDto> ToggleActiveStatusAsync(int id);
    }

 
}
