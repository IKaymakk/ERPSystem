namespace ERPSystem.Core.DTOs.Unit;

public class UnitSelectDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }

    // Display için birleştirilmiş format: "Kilogram (kg)"
    public string DisplayText => $"{Name} ({Symbol})";
}