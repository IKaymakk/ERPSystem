using ERPSystem.Core.DTOs.Common;

namespace ERPSystem.Core.DTOs.Product
{
    public class ProductFilterDto : PagedRequestDto
    {
        public int? CategoryId { get; set; }
        public int? UnitId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLowStock { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Barcode { get; set; }
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
    }
}
