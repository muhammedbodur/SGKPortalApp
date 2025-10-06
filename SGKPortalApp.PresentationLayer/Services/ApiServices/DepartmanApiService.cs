using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices
{
    public class DepartmanApiService : IDepartmanApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DepartmanApiService> _logger;

        public DepartmanApiService(HttpClient httpClient, ILogger<DepartmanApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<DepartmanResponseDto>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/departman");
            if (!response.IsSuccessStatusCode)
                return ServiceResult<List<DepartmanResponseDto>>.Fail("Departman listesi alınamadı.");

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DepartmanResponseDto>>>();
            return apiResponse?.Success == true
                ? ServiceResult<List<DepartmanResponseDto>>.Ok(apiResponse.Data)
                : ServiceResult<List<DepartmanResponseDto>>.Fail(apiResponse?.Message ?? "Bilinmeyen hata");
        }

        public async Task<ServiceResult<DepartmanResponseDto>> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/departman/{id}");
            if (!response.IsSuccessStatusCode)
                return ServiceResult<DepartmanResponseDto>.Fail("Departman bulunamadı.");

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanResponseDto>>();
            return apiResponse?.Success == true
                ? ServiceResult<DepartmanResponseDto>.Ok(apiResponse.Data)
                : ServiceResult<DepartmanResponseDto>.Fail(apiResponse?.Message ?? "Bilinmeyen hata");
        }

        public async Task<ServiceResult<DepartmanResponseDto>> CreateAsync(DepartmanCreateRequestDto request)
        {
            try
            {
                _logger.LogInformation("🔵 CreateAsync çağrıldı");
                _logger.LogInformation("📦 Request: {@Request}", request);

                var response = await _httpClient.PostAsJsonAsync("api/departman", request);

                // ⭐ HAta detayını oku
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("📡 Status: {Status}, Content: {Content}",
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ API Hatası: {Error}", responseContent);
                    return ServiceResult<DepartmanResponseDto>.Fail(
                        $"Departman eklenemedi. Detay: {responseContent}");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanResponseDto>>();
                return apiResponse?.Success == true
                    ? ServiceResult<DepartmanResponseDto>.Ok(apiResponse.Data)
                    : ServiceResult<DepartmanResponseDto>.Fail(apiResponse?.Message ?? "Bilinmeyen hata");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CreateAsync Exception");
                return ServiceResult<DepartmanResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }


        public async Task<ServiceResult<DepartmanResponseDto>> UpdateAsync(int id, DepartmanUpdateRequestDto request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/departman/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ServiceResult<DepartmanResponseDto>.Fail("Departman güncellenemedi.");

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanResponseDto>>();
            return apiResponse?.Success == true
                ? ServiceResult<DepartmanResponseDto>.Ok(apiResponse.Data)
                : ServiceResult<DepartmanResponseDto>.Fail(apiResponse?.Message ?? "Bilinmeyen hata");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/departman/{id}");
            if (!response.IsSuccessStatusCode)
                return ServiceResult<bool>.Fail("Departman silinemedi.");

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
            return apiResponse?.Success == true
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.Fail(apiResponse?.Message ?? "Bilinmeyen hata");
        }

        public async Task<ServiceResult<List<DepartmanResponseDto>>> GetActiveAsync()
        {
            var response = await _httpClient.GetAsync("api/departman/active");
            if (!response.IsSuccessStatusCode)
                return ServiceResult<List<DepartmanResponseDto>>.Fail("Aktif departmanlar alınamadı.");

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DepartmanResponseDto>>>();
            return apiResponse?.Success == true
                ? ServiceResult<List<DepartmanResponseDto>>.Ok(apiResponse.Data)
                : ServiceResult<List<DepartmanResponseDto>>.Fail(apiResponse?.Message ?? "Bilinmeyen hata");
        }

        public async Task<ServiceResult<PagedResponseDto<DepartmanResponseDto>>> GetPagedAsync(DepartmanFilterRequestDto filter)
        {
            var response = await _httpClient.PostAsJsonAsync("api/departman/paged", filter);
            if (!response.IsSuccessStatusCode)
                return ServiceResult<PagedResponseDto<DepartmanResponseDto>>.Fail("Sayfalı liste alınamadı.");

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResponseDto<DepartmanResponseDto>>>();
            return apiResponse?.Success == true
                ? ServiceResult<PagedResponseDto<DepartmanResponseDto>>.Ok(apiResponse.Data)
                : ServiceResult<PagedResponseDto<DepartmanResponseDto>>.Fail(apiResponse?.Message ?? "Bilinmeyen hata");
        }
    }
}