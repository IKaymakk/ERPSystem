using ERPSystem.Core.DTOs.Common;

namespace ERPSystem.Core.DTOs.Category;

public class CategoryFilterDto : PagedRequestDto
{
    //  Temel Arama
    public string Code { get; set; } // "ELK" içeren kodları ara
    public string Name { get; set; } // "Bilgi" içeren isimleri ara

    //  Hiyerarşi Filtreleri
    public int? ParentCategoryId { get; set; } // Sadece bu parent'ın altlarını getir
    public bool? IsActive { get; set; } // Aktif/pasif kategoriler

    //  Tarih Filtreleri
    public DateTime? CreatedDateFrom { get; set; } // Bu tarihten sonra olanlar
    public DateTime? CreatedDateTo { get; set; } // Bu tarihten önce olanlar

    //  Özel Filtreler
    public bool IncludeInactive { get; set; } = false; // Pasif kategorileri de getir mi?
    public bool LoadHierarchy { get; set; } = false; // Children'ları da yükle mi?
    public int? MaxLevel { get; set; } // Maksimum kaç seviye derinlik getir?
}