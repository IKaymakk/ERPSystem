namespace ERPSystem.Core.DTOs.Product
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int UnitId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal MinStockLevel { get; set; }
        public string Barcode { get; set; }
        public decimal VatRate { get; set; }
        public string ImagePath { get; set; }
        public bool IsActive { get; set; }
    }
}
