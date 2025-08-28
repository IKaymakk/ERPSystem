namespace ERPSystem.Core.DTOs.Product
{
    public class CreateProductDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int UnitId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CurrentStock { get; set; } = 0;
        public decimal MinStockLevel { get; set; } = 0;
        public string Barcode { get; set; }
        public decimal VatRate { get; set; } = 18.00m;
        public string ImagePath { get; set; }
    }
}
