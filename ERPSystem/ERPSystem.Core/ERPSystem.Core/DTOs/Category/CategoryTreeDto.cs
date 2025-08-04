namespace ERPSystem.Core.DTOs.Category;

public class CategoryTreeDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int Level { get; set; } // Indentation için
    public bool HasChildren { get; set; } // + - ikonları için
    public int ProductCount { get; set; } // "15 ürün" badge'i için
    public bool IsActive { get; set; }
    public List<CategoryTreeDto> Children { get; set; } = new List<CategoryTreeDto>();
}