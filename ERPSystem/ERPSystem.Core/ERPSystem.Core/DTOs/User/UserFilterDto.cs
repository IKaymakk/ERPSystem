using ERPSystem.Core.DTOs.Common;

public class UserFilterDto : PagedRequestDto
{
    public int? RoleId { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
}