namespace ERPSystem.Core.DTOs.Product
{
    public class ProductStockInfoDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal MinStockLevel { get; set; }
        public bool IsLowStock { get; set; }
        public string UnitSymbol { get; set; }
    }
}
