namespace ERPSystem.Core.DTOs.Unit;

public class DeleteUnitResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int ProductCount { get; set; } // Kaç üründe kullanıldığı bilgisi

    // Factory methods
    public static DeleteUnitResultDto SuccessResult()
    {
        return new DeleteUnitResultDto
        {
            Success = true,
            Message = "Unit başarıyla silindi",
            ProductCount = 0
        };
    }

    public static DeleteUnitResultDto FailedResult(string message, int productCount = 0)
    {
        return new DeleteUnitResultDto
        {
            Success = false,
            Message = message,
            ProductCount = productCount
        };
    }
}