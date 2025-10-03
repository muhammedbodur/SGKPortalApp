using Microsoft.AspNetCore.Components;
using SGKPortalApp.PresentationLayer.Services.UIServices;

namespace SGKPortalApp.PresentationLayer.Shared.Common
{
    /// <summary>
    /// Toast bildirim component'i
    /// Başarı, hata, uyarı ve bilgi mesajlarını gösterir
    /// </summary>
    public partial class ToastBase : ComponentBase, IDisposable
    {
        #region Dependencies

        /// <summary>
        /// Toast servisi - Dependency Injection ile gelir
        /// </summary>
        [Inject]
        public IToastService ToastService { get; set; } = default!;

        #endregion

        #region Properties

        /// <summary>
        /// Aktif olarak ekranda gösterilen toast'lar
        /// </summary>
        protected List<ToastMessage> ActiveToasts { get; set; } = new();

        /// <summary>
        /// Eski toast'ları temizlemek için timer
        /// </summary>
        private Timer? _cleanupTimer;

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Component ilk yüklendiğinde çalışır
        /// ToastService'e abone olur ve cleanup timer'ı başlatır
        /// </summary>
        protected override void OnInitialized()
        {
            // ToastService'in OnShow event'ine abone ol
            ToastService.OnShow += HandleToastShow;

            // Her 1 saniyede bir eski toast'ları temizle
            _cleanupTimer = new Timer(
                callback: _ => CleanupOldToasts(),
                state: null,
                dueTime: TimeSpan.FromSeconds(1),
                period: TimeSpan.FromSeconds(1)
            );
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Yeni bir toast mesajı geldiğinde çalışır
        /// </summary>
        /// <param name="toast">Gösterilecek toast mesajı</param>
        private void HandleToastShow(ToastMessage toast)
        {
            // Listeye ekle
            ActiveToasts.Add(toast);

            // Duration süresi sonra otomatik kaldır
            Task.Delay(toast.Duration).ContinueWith(_ =>
            {
                RemoveToast(toast);
            });

            // UI'ı güncelle
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Toast mesajını listeden kaldırır
        /// </summary>
        /// <param name="toast">Kaldırılacak toast</param>
        protected void RemoveToast(ToastMessage toast)
        {
            if (ActiveToasts.Contains(toast))
            {
                ActiveToasts.Remove(toast);
                InvokeAsync(StateHasChanged);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Süresi dolmuş toast'ları otomatik temizler
        /// Timer tarafından her saniye çağrılır
        /// </summary>
        private void CleanupOldToasts()
        {
            var expiredToasts = ActiveToasts
                .Where(t => DateTime.Now - t.Timestamp > TimeSpan.FromMilliseconds(t.Duration))
                .ToList();

            if (expiredToasts.Any())
            {
                foreach (var toast in expiredToasts)
                {
                    ActiveToasts.Remove(toast);
                }
                InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Toast tipine göre CSS class döndürür
        /// </summary>
        /// <param name="type">Toast tipi</param>
        /// <returns>Bootstrap CSS class</returns>
        protected string GetToastClass(ToastType type)
        {
            return type switch
            {
                ToastType.Success => "bg-success text-white",
                ToastType.Error => "bg-danger text-white",
                ToastType.Warning => "bg-warning text-dark",
                ToastType.Info => "bg-info text-white",
                _ => "bg-secondary text-white"
            };
        }

        /// <summary>
        /// Toast header için CSS class döndürür
        /// </summary>
        /// <param name="type">Toast tipi</param>
        /// <returns>Bootstrap CSS class</returns>
        protected string GetHeaderClass(ToastType type)
        {
            return type switch
            {
                ToastType.Success => "bg-success text-white",
                ToastType.Error => "bg-danger text-white",
                ToastType.Warning => "bg-warning text-dark",
                ToastType.Info => "bg-info text-white",
                _ => "bg-secondary text-white"
            };
        }

        /// <summary>
        /// Toast tipine göre uygun icon class döndürür
        /// Boxicons kullanılıyor
        /// </summary>
        /// <param name="type">Toast tipi</param>
        /// <returns>Boxicon CSS class</returns>
        protected string GetIconClass(ToastType type)
        {
            return type switch
            {
                ToastType.Success => "bx bx-check-circle",
                ToastType.Error => "bx bx-error-circle",
                ToastType.Warning => "bx bx-error",
                ToastType.Info => "bx bx-info-circle",
                _ => "bx bx-bell"
            };
        }

        /// <summary>
        /// Zaman farkını Türkçe metin olarak döndürür
        /// Örnek: "şimdi", "5 dk önce", "2 sa önce"
        /// </summary>
        /// <param name="timestamp">Zaman damgası</param>
        /// <returns>Türkçe zaman metni</returns>
        protected string GetTimeAgo(DateTime timestamp)
        {
            var elapsed = DateTime.Now - timestamp;

            if (elapsed.TotalSeconds < 60)
                return "şimdi";
            else if (elapsed.TotalMinutes < 60)
                return $"{(int)elapsed.TotalMinutes} dk önce";
            else if (elapsed.TotalHours < 24)
                return $"{(int)elapsed.TotalHours} sa önce";
            else
                return timestamp.ToString("dd.MM.yyyy HH:mm");
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Component dispose edildiğinde çalışır
        /// Memory leak'i önlemek için event ve timer'ı temizler
        /// </summary>
        public void Dispose()
        {
            // Event aboneliğini iptal et
            if (ToastService != null)
            {
                ToastService.OnShow -= HandleToastShow;
            }

            // Timer'ı durdur ve temizle
            _cleanupTimer?.Dispose();
        }

        #endregion
    }
}