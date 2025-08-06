namespace ERPSystem.Core.DTOs.Unit;

public class BulkOperationResultDto<T>
{
    public IEnumerable<T> SuccessfulItems { get; set; } = new List<T>();
    public IEnumerable<BulkOperationErrorDto> FailedItems { get; set; } = new List<BulkOperationErrorDto>();
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public bool IsCompletelySuccessful => FailureCount == 0;
    public bool HasPartialSuccess => SuccessCount > 0 && FailureCount > 0;
}