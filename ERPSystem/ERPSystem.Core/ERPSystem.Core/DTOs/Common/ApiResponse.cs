// Core/DTOs/Common/ApiResponse.cs
namespace ERPSystem.Core.DTOs.Common
{
    /// <summary>
    /// API'den dönen standart response formatı
    /// </summary>
    /// <typeparam name="T">Response data tipi</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// İşlem başarılı mı?
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// İşlem mesajı
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Response data'sı
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Hata detayları (varsa)
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Response oluşturma zamanı
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Constructors
        public ApiResponse()
        {
        }

        public ApiResponse(bool isSuccess, string message, T? data = default, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            Errors = errors;
        }

        // Static factory methods
        /// <summary>
        /// Başarılı response oluşturur
        /// </summary>
        public static ApiResponse<T> Success(T? data, string message = "İşlem başarılı")
        {
            return new ApiResponse<T>(true, message, data);
        }

        /// <summary>
        /// Başarılı response oluşturur (sadece mesaj ile)
        /// </summary>
        public static ApiResponse<T> Success(string message)
        {
            return new ApiResponse<T>(true, message);
        }

        /// <summary>
        /// Hatalı response oluşturur
        /// </summary>
        public static ApiResponse<T> Failure(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, default, errors);
        }

        /// <summary>
        /// Hatalı response oluşturur (tek hata ile)
        /// </summary>
        public static ApiResponse<T> Failure(string message, string error)
        {
            return new ApiResponse<T>(false, message, default, new List<string> { error });
        }
    }

    /// <summary>
    /// Data içermeyen response için
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse() : base()
        {
        }

        public ApiResponse(bool isSuccess, string message, List<string>? errors = null)
            : base(isSuccess, message, null, errors)
        {
        }

        // Static factory methods
        public static ApiResponse Success(string message = "İşlem başarılı")
        {
            return new ApiResponse(true, message);
        }

        public static ApiResponse Failure(string message, List<string>? errors = null)
        {
            return new ApiResponse(false, message, errors);
        }

        public static ApiResponse Failure(string message, string error)
        {
            return new ApiResponse(false, message, new List<string> { error });
        }
    }
}