namespace ERPSystem.Core.DTOs.Common;

public class PagedRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchTerm { get; set; } = string.Empty;
    public string SortBy { get; set; } = string.Empty; 
    public bool SortDescending { get; set; } = false;
}