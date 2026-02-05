using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
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

        // ─── CRUD ───────────────────────────────────────────

        public async Task<ApiResponseDto<HaberListeResponseDto>> GetAdminHaberListeAsync(int page = 1, int pageSize = 12, string? search = null)
        {
            try
            {
                var url = $"{BaseUrl}/admin?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrWhiteSpace(search))
                    url += $"&search={Uri.EscapeDataString(search)}";

                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<HaberListeResponseDto>>(url);
                return response ?? ApiResponseDto<HaberListeResponseDto>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin haber listesi getirilirken hata oluştu");
                return ApiResponseDto<HaberListeResponseDto>.ErrorResult("Admin haber listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponseDto<HaberResponseDto>> CreateHaberAsync(HaberCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<HaberResponseDto>>()
                    ?? ApiResponseDto<HaberResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber oluşturulurken hata oluştu");
                return ApiResponseDto<HaberResponseDto>.ErrorResult($"Haber oluşturulurken hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<HaberResponseDto>> UpdateHaberAsync(int haberId, HaberUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{haberId}", request);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<HaberResponseDto>>()
                    ?? ApiResponseDto<HaberResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber güncellenirken hata oluştu");
                return ApiResponseDto<HaberResponseDto>.ErrorResult($"Haber güncellenirken hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteHaberAsync(int haberId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{haberId}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>()
                    ?? ApiResponseDto<bool>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber silinirken hata oluştu");
                return ApiResponseDto<bool>.ErrorResult($"Haber silinirken hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<HaberResimResponseDto>> AddResimAsync(int haberId, HaberResimCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{haberId}/resim", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<HaberResimResponseDto>>()
                    ?? ApiResponseDto<HaberResimResponseDto>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber resimi eklenirken hata oluştu");
                return ApiResponseDto<HaberResimResponseDto>.ErrorResult($"Resim eklenirken hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteResimAsync(int haberId, int resimId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{haberId}/resim/{resimId}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>()
                    ?? ApiResponseDto<bool>.ErrorResult("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Haber resimi silinirken hata oluştu");
                return ApiResponseDto<bool>.ErrorResult($"Resim silinirken hata: {ex.Message}");
            }
        }
    }
}
