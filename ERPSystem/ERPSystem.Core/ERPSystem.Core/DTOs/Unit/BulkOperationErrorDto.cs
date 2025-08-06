namespace ERPSystem.Core.DTOs.Unit;

public class BulkOperationErrorDto
{
    public int Index { get; set; }
    public string Data { get; set; }
    public Dictionary<string, string[]> ValidationErrors { get; set; } = new();
    public string ErrorMessage { get; set; }
}