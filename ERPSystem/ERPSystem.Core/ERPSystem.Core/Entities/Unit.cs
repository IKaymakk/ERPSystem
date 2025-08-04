namespace ERPSystem.Core.Entities;

public class Unit : BaseEntity
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public string Description { get; set; }

    public virtual List<Product> Products { get; set; } = new List<Product>();
}