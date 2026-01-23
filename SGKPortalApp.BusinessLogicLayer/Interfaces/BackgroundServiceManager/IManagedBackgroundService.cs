namespace SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager
{
    /// <summary>
    /// Yönetilebilir background service interface'i
    /// Bu interface'i implement eden servisler BackgroundServiceManager tarafından yönetilebilir
    /// </summary>
    public interface IManagedBackgroundService
    {
        /// <summary>
        /// Servis adı (unique identifier)
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Görüntüleme adı
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Servis çalışıyor mu?
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Servis duraklatıldı mı?
        /// </summary>
        bool IsPaused { get; set; }

        /// <summary>
        /// Son çalışma zamanı
        /// </summary>
        DateTime? LastRunTime { get; }

        /// <summary>
        /// Sonraki çalışma zamanı
        /// </summary>
        DateTime? NextRunTime { get; }

        /// <summary>
        /// Çalışma aralığı
        /// </summary>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// Son hata mesajı
        /// </summary>
        string? LastError { get; }

        /// <summary>
        /// Başarılı çalışma sayısı
        /// </summary>
        int SuccessCount { get; }

        /// <summary>
        /// Hatalı çalışma sayısı
        /// </summary>
        int ErrorCount { get; }

        /// <summary>
        /// Servisi manuel olarak tetikler
        /// </summary>
        Task TriggerAsync(CancellationToken cancellationToken = default);
    }
}
