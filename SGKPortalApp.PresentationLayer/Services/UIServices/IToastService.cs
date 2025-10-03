namespace SGKPortalApp.PresentationLayer.Services.UIServices
{
    /// <summary>
    /// Toast bildirim servisi arayüzü
    /// Kullanıcıya başarı, hata, uyarı ve bilgi mesajları gösterir
    /// </summary>
    public interface IToastService
    {
        /// <summary>
        /// Toast mesajı gösterildiğinde tetiklenen event
        /// Toast.razor component'i bu event'i dinler
        /// </summary>
        event Action<ToastMessage>? OnShow;

        /// <summary>
        /// Başarı mesajı gösterir (Yeşil renk)
        /// </summary>
        /// <param name="message">Gösterilecek mesaj</param>
        /// <param name="title">Başlık (opsiyonel)</param>
        Task ShowSuccessAsync(string message, string? title = null);

        /// <summary>
        /// Hata mesajı gösterir (Kırmızı renk)
        /// </summary>
        /// <param name="message">Gösterilecek mesaj</param>
        /// <param name="title">Başlık (opsiyonel)</param>
        Task ShowErrorAsync(string message, string? title = null);

        /// <summary>
        /// Uyarı mesajı gösterir (Sarı renk)
        /// </summary>
        /// <param name="message">Gösterilecek mesaj</param>
        /// <param name="title">Başlık (opsiyonel)</param>
        Task ShowWarningAsync(string message, string? title = null);

        /// <summary>
        /// Bilgi mesajı gösterir (Mavi renk)
        /// </summary>
        /// <param name="message">Gösterilecek mesaj</param>
        /// <param name="title">Başlık (opsiyonel)</param>
        Task ShowInfoAsync(string message, string? title = null);
    }

    /// <summary>
    /// Toast mesaj modeli
    /// Her bir bildirim için gerekli bilgileri tutar
    /// </summary>
    public class ToastMessage
    {
        /// <summary>
        /// Gösterilecek ana mesaj
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Toast başlığı (opsiyonel)
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Toast tipi (Success, Error, Warning, Info)
        /// </summary>
        public ToastType Type { get; set; }

        /// <summary>
        /// Mesajın oluşturulma zamanı
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Toast'un ekranda kalma süresi (milisaniye)
        /// Varsayılan: 5000ms (5 saniye)
        /// </summary>
        public int Duration { get; set; } = 5000;
    }

    /// <summary>
    /// Toast mesaj tipleri
    /// Her tip farklı renk ve icon ile gösterilir
    /// </summary>
    public enum ToastType
    {
        /// <summary>
        /// Başarılı işlem (Yeşil, check icon)
        /// </summary>
        Success = 1,

        /// <summary>
        /// Hata durumu (Kırmızı, error icon)
        /// </summary>
        Error = 2,

        /// <summary>
        /// Uyarı mesajı (Sarı, warning icon)
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Bilgilendirme (Mavi, info icon)
        /// </summary>
        Info = 4
    }
}