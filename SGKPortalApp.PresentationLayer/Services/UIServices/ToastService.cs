namespace SGKPortalApp.PresentationLayer.Services.UIServices
{
    /// <summary>
    /// Toast bildirim servisi - Kullanıcıya mesaj gösterme servisi
    /// Başarı, hata, uyarı ve bilgi mesajları gösterir
    /// </summary>
    public class ToastService : IToastService
    {
        /// <summary>
        /// Toast mesajı gösterildiğinde tetiklenen event
        /// </summary>
        public event Action<ToastMessage>? OnShow;

        /// <summary>
        /// Başarı mesajı gösterir (Yeşil renk)
        /// </summary>
        /// <param name="message">Gösterilecek mesaj</param>
        /// <param name="title">Başlık (opsiyonel, varsayılan: "Başarılı")</param>
        public async Task ShowSuccessAsync(string message, string? title = null)
        {
            await ShowAsync(message, title ?? "Başarılı", ToastType.Success);
        }

        /// <summary>
        /// Hata mesajı gösterir (Kırmızı renk)
        /// </summary>
        /// <param name="message">Gösterilecek mesaj</param>
        /// <param name="title">Başlık (opsiyonel, varsayılan: "Hata")</param>
        public async Task ShowErrorAsync(string message, string? title = null)
        {
            await ShowAsync(message, title ?? "Hata", ToastType.Error);
        }

        /// <summary>
        /// Uyarı mesajı gösterir (Sarı renk)
        /// </summary>
        /// <param name="message">Gösterilecek mesaj</param>
        /// <param name="title">Başlık (opsiyonel, varsayılan: "Uyarı")</param>
        public async Task ShowWarningAsync(string message, string? title = null)
        {
            await ShowAsync(message, title ?? "Uyarı", ToastType.Warning);
        }

        /// <summary>
        /// Bilgi mesajı gösterir (Mavi renk)
        /// </summary>
        /// <param name="message">Gösterilecek mesaj</param>
        /// <param name="title">Başlık (opsiyonel, varsayılan: "Bilgi")</param>
        public async Task ShowInfoAsync(string message, string? title = null)
        {
            await ShowAsync(message, title ?? "Bilgi", ToastType.Info);
        }

        /// <summary>
        /// Toast mesajını gösterir (İç metod)
        /// </summary>
        /// <param name="message">Gösterilecek mesaj</param>
        /// <param name="title">Başlık</param>
        /// <param name="type">Toast tipi (Success, Error, Warning, Info)</param>
        private async Task ShowAsync(string message, string? title, ToastType type)
        {
            // Mesaj boş ise hata fırlat
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Toast mesajı boş olamaz!", nameof(message));
            }

            // Toast mesaj nesnesini oluştur
            var toast = new ToastMessage
            {
                Message = message.Trim(),
                Title = title?.Trim(),
                Type = type,
                Timestamp = DateTime.Now,
                // Hata mesajları daha uzun süre ekranda kalır (7 saniye)
                // Diğer mesajlar 5 saniye kalır
                Duration = type == ToastType.Error ? 7000 : 5000
            };

            // Event'e abone olan var mı kontrol et
            if (OnShow != null)
            {
                // UI thread'i bloklamadan çalışması için Task.Yield kullanıyoruz
                // Bu, Blazor'un StateHasChanged metodunu doğru şekilde tetiklemesini sağlar
                await Task.Yield();

                // Tüm abonelere event'i gönder
                OnShow.Invoke(toast);
            }
        }
    }
}