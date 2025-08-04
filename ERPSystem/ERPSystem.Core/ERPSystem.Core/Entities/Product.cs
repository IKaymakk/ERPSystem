using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSystem.Core.Entities;

public class Product : BaseEntity
{
    public string Code { get; set; }
    [StringLength(200)]
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public int UnitId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SalePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentStock { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinStockLevel { get; set; }
    public string Barcode { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal VatRate { get; set; } = 18.00m;
    public string ImagePath { get; set; }

    public virtual Category Category { get; set; }
    public virtual Unit Unit { get; set; }
    public virtual List<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}