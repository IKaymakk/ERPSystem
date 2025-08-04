using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSystem.Core.Entities;

public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    public string MovementType { get; set; } // IN (Giriş), OUT (Çıkış), ADJUSTMENT (Düzeltme)

    [Column(TypeName = "decimal(18,3)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    public string DocumentType { get; set; } // SALE, PURCHASE, ADJUSTMENT
    public int? DocumentId { get; set; }
    public string Description { get; set; }
    public DateTime MovementDate { get; set; } = DateTime.Now;

    public virtual Product Product { get; set; }
}