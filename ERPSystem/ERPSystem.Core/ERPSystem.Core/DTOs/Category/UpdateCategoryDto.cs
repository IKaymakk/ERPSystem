namespace ERPSystem.Core.DTOs.Category;

public class UpdateCategoryDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}