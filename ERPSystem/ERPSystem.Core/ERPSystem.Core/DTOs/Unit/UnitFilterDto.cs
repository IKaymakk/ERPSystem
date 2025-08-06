using ERPSystem.Core.DTOs.Common;

namespace ERPSystem.Core.DTOs.Unit;

public class UnitFilterDto : PagedRequestDto
{
    public bool? IsActive { get; set; }

    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
}