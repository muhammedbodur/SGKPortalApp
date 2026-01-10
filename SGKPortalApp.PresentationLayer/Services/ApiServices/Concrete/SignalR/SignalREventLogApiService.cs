using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.SignalR;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.SignalR
{
    /// <summary>
    /// SignalR Event Log API Service Implementation
    /// </summary>
    public class SignalREventLogApiService : ISignalREventLogApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SignalREventLogApiService> _logger;

        public SignalREventLogApiService(IHttpClientFactory httpClientFactory, ILogger<SignalREventLogApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<PagedResultDto<SignalREventLogResponseDto>?> GetFilteredAsync(SignalREventLogFilterDto filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("signalr/eventlog/filter", filter);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PagedResultDto<SignalREventLogResponseDto>>();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFilteredAsync hatası");
                return null;
            }
        }

        public async Task<List<SignalREventLogResponseDto>> GetRecentAsync(int minutes = 5)
        {
            try
            {
                var response = await _httpClient.GetAsync($"signalr/eventlog/recent/{minutes}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<SignalREventLogResponseDto>>() ?? new();
                }
                return new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRecentAsync hatası");
                return new();
            }
        }

        public async Task<List<SignalREventLogResponseDto>> GetBySiraAsync(int siraId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"signalr/eventlog/by-sira/{siraId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<SignalREventLogResponseDto>>() ?? new();
                }
                return new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBySiraAsync hatası: {SiraId}", siraId);
                return new();
            }
        }

        public async Task<SignalREventLogStatsDto?> GetStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = "signalr/eventlog/stats";
                var queryParams = new List<string>();
                
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-ddTHH:mm:ss}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-ddTHH:mm:ss}");
                
                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SignalREventLogStatsDto>();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStatsAsync hatası");
                return null;
            }
        }
    }
}
