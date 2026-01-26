using SGKPortalApp.BusinessObjectLayer.DTOs.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IBackgroundServiceApiService
    {
        Task<ServiceResult<List<BackgroundServiceStatusDto>>> GetAllAsync();
        Task<ServiceResult<BackgroundServiceStatusDto>> GetAsync(string serviceName);
        Task<ServiceResult<string>> TriggerAsync(string serviceName);
        Task<ServiceResult<string>> PauseAsync(string serviceName);
        Task<ServiceResult<string>> ResumeAsync(string serviceName);
        Task<ServiceResult<string>> SetIntervalAsync(string serviceName, int intervalMinutes);
    }

    public class BackgroundServiceStatusDto
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
