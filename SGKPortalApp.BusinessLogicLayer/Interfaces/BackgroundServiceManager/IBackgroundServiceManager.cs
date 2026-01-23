namespace SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager
{
    public interface IBackgroundServiceManager
    {
        /// <summary>
        /// Tüm kayıtlı servislerin durumlarını getirir
        /// </summary>
        IEnumerable<BackgroundServiceStatus> GetAllServiceStatuses();

        /// <summary>
        /// Belirli bir servisin durumunu getirir
        /// </summary>
        BackgroundServiceStatus? GetServiceStatus(string serviceName);

        /// <summary>
        /// Servisi manuel olarak tetikler (hemen çalıştırır)
        /// </summary>
        Task<bool> TriggerServiceAsync(string serviceName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Servisi duraklatır (bir sonraki periyodik çalışmayı atlar)
        /// </summary>
        bool PauseService(string serviceName);

        /// <summary>
        /// Duraklatılmış servisi devam ettirir
        /// </summary>
        bool ResumeService(string serviceName);

        /// <summary>
        /// Servisin sync aralığını değiştirir
        /// </summary>
        bool SetServiceInterval(string serviceName, TimeSpan interval);
    }

    public class BackgroundServiceStatus
    {
        public string ServiceName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }
        public DateTime? LastRunTime { get; set; }
        public DateTime? NextRunTime { get; set; }
        public TimeSpan Interval { get; set; }
        public string? LastError { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
    }
}
