namespace ERPSystem.Core.DTOs.Unit;

public class UnitValidationResultDto
{
    public bool IsValid { get; set; }
    public Dictionary<string, string> Errors { get; set; } = new();

    public void AddError(string field, string message)
    {
        if (Errors.ContainsKey(field))
        {
            Errors[field] += $"; {message}";
        }
        else
        {
            Errors[field] = message;
        }
        IsValid = false;
    }

    public static UnitValidationResultDto Success()
    {
        return new UnitValidationResultDto { IsValid = true };
    }
}