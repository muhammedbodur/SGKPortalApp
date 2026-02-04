using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class HaberApiService : IHaberApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HaberApiService> _logger;
        private const string BaseUrl = "haberler";

        public HaberApiService(HttpClient httpClient, ILogger<HaberApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<HaberResponseDto>>> GetSliderHaberleriAsync(int count = 5)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<HaberResponseDto>>>($"{BaseUrl}/slider?count={count}");
                return response ?? ApiResponseDto<List<HaberResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slider haberleri getirilirken hata oluştu");
                return ApiResponseDto<List<HaberResponseDto>>.ErrorResult("Slider haberleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<HaberListeResponseDto>> GetHaberListeAsync(int page = 1, int pageSize = 12, string? search = null)
        {
            try
            {
                var url = $"{BaseUrl}?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrWhiteSpace(search))
                    url += $"&search={Uri.EscapeDataString(search)}";

                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<HaberListeResponseDto>>(url);
                return response ?? ApiResponseDto<HaberListeResponseDto>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber listesi getirilirken hata oluştu");
                return ApiResponseDto<HaberListeResponseDto>.ErrorResult("Haber listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<HaberResponseDto?>> GetHaberByIdAsync(int haberId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<HaberResponseDto?>>($"{BaseUrl}/{haberId}");
                return response ?? ApiResponseDto<HaberResponseDto?>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber detay getirilirken hata oluştu");
                return ApiResponseDto<HaberResponseDto?>.ErrorResult("Haber detay getirilirken hata oluştu");
            }
        }

        public async Task<byte[]?> DownloadHaberWordAsync(int haberId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{haberId}/word");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsByteArrayAsync();
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber Word dosyası indirilirken hata oluştu");
                return null;
            }
        }
    }
}
