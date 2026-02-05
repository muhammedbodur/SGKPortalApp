using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    /// <summary>
    /// Dashboard API Service Implementation
    /// </summary>
    public class DashboardApiService : IDashboardApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DashboardApiService> _logger;
        private const string BaseUrl = "dashboard";

        public DashboardApiService(HttpClient httpClient, ILogger<DashboardApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponseDto<DashboardDataResponseDto>> GetDashboardDataAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<DashboardDataResponseDto>>(BaseUrl);
                return response ?? ApiResponseDto<DashboardDataResponseDto>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard verileri getirilirken hata oluştu");
                return ApiResponseDto<DashboardDataResponseDto>.ErrorResult("Dashboard verileri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<HaberDashboardResponseDto>>> GetSliderHaberleriAsync(int count = 5)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<HaberDashboardResponseDto>>>($"{BaseUrl}/slider-haberler?count={count}");
                return response ?? ApiResponseDto<List<HaberDashboardResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slider haberleri getirilirken hata oluştu");
                return ApiResponseDto<List<HaberDashboardResponseDto>>.ErrorResult("Slider haberleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<HaberDashboardResponseDto>>> GetListeHaberleriAsync(int count = 10)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<HaberDashboardResponseDto>>>($"{BaseUrl}/liste-haberler?count={count}");
                return response ?? ApiResponseDto<List<HaberDashboardResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Liste haberleri getirilirken hata oluştu");
                return ApiResponseDto<List<HaberDashboardResponseDto>>.ErrorResult("Liste haberleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<SikKullanilanProgramResponseDto>>> GetSikKullanilanProgramlarAsync(int count = 8)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<SikKullanilanProgramResponseDto>>>($"{BaseUrl}/sik-kullanilan-programlar?count={count}");
                return response ?? ApiResponseDto<List<SikKullanilanProgramResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sık kullanılan programlar getirilirken hata oluştu");
                return ApiResponseDto<List<SikKullanilanProgramResponseDto>>.ErrorResult("Sık kullanılan programlar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<OnemliLinkResponseDto>>> GetOnemliLinklerAsync(int count = 10)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<OnemliLinkResponseDto>>>($"{BaseUrl}/onemli-linkler?count={count}");
                return response ?? ApiResponseDto<List<OnemliLinkResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Önemli linkler getirilirken hata oluştu");
                return ApiResponseDto<List<OnemliLinkResponseDto>>.ErrorResult("Önemli linkler getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<GununMenusuResponseDto?>> GetGununMenusuAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<GununMenusuResponseDto?>>($"{BaseUrl}/gunun-menusu");
                return response ?? ApiResponseDto<GununMenusuResponseDto?>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Günün menüsü getirilirken hata oluştu");
                return ApiResponseDto<GununMenusuResponseDto?>.ErrorResult("Günün menüsü getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<List<BugunDoganResponseDto>>> GetBugunDoganlarAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<BugunDoganResponseDto>>>($"{BaseUrl}/bugun-doganlar");
                return response ?? ApiResponseDto<List<BugunDoganResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bugün doğanlar getirilirken hata oluştu");
                return ApiResponseDto<List<BugunDoganResponseDto>>.ErrorResult("Bugün doğanlar getirilirken hata oluştu");
            }
        }
    }
}
