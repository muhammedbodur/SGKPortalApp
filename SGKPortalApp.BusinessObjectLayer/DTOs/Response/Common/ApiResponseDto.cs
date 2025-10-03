namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Genel API yanıt wrapper DTO - Tüm API çağrılarında kullanılır
    /// </summary>
    /// <typeparam name="T">Response data tipi</typeparam>
    public class ApiResponseDto<T>
    {
        /// <summary>
        /// İşlem başarılı mı?
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Başarı/Hata mesajı
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Response data
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Hata listesi (varsa)
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Başarılı response oluştur
        /// </summary>
        public static ApiResponseDto<T> SuccessResult(T data, string? message = null)
        {
            return new ApiResponseDto<T>
            {
                Success = true,
                Message = message ?? "İşlem başarılı",
                Data = data,
                Errors = null
            };
        }

        /// <summary>
        /// Hatalı response oluştur
        /// </summary>
        public static ApiResponseDto<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>()
            };
        }

        /// <summary>
        /// Hatalı response oluştur (tek hata mesajı)
        /// </summary>
        public static ApiResponseDto<T> ErrorResult(string message, string error)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = new List<string> { error }
            };
        }
    }
}