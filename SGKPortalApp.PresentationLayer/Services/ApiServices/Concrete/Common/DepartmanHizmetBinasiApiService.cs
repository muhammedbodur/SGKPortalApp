using System.Net.Http.Json;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class DepartmanHizmetBinasiApiService : IDepartmanHizmetBinasiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DepartmanHizmetBinasiApiService> _logger;
        private const string BaseUrl = "DepartmanHizmetBinasi";

        public DepartmanHizmetBinasiApiService(
            HttpClient httpClient,
            ILogger<DepartmanHizmetBinasiApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>>(BaseUrl);
                return response ?? ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync hatası");
                return ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<DepartmanHizmetBinasiResponseDto>>($"{BaseUrl}/{id}");
                return response ?? ApiResponseDto<DepartmanHizmetBinasiResponseDto>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync hatası. Id: {Id}", id);
                return ApiResponseDto<DepartmanHizmetBinasiResponseDto>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetByDepartmanAsync(int departmanId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>>($"{BaseUrl}/departman/{departmanId}");
                return response ?? ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByDepartmanAsync hatası. DepartmanId: {DepartmanId}", departmanId);
                return ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>>($"{BaseUrl}/hizmet-binasi/{hizmetBinasiId}");
                return response ?? ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByHizmetBinasiAsync hatası. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<DepartmanHizmetBinasiResponseDto>>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanHizmetBinasiDto>>>($"{BaseUrl}/dropdown");
                return response ?? ApiResponseDto<List<DepartmanHizmetBinasiDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDropdownAsync hatası");
                return ApiResponseDto<List<DepartmanHizmetBinasiDto>>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownByDepartmanAsync(int departmanId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanHizmetBinasiDto>>>($"{BaseUrl}/dropdown/departman/{departmanId}");
                return response ?? ApiResponseDto<List<DepartmanHizmetBinasiDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDropdownByDepartmanAsync hatası. DepartmanId: {DepartmanId}", departmanId);
                return ApiResponseDto<List<DepartmanHizmetBinasiDto>>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<DepartmanHizmetBinasiDto>>> GetDropdownByHizmetBinasiAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanHizmetBinasiDto>>>($"{BaseUrl}/dropdown/hizmet-binasi/{hizmetBinasiId}");
                return response ?? ApiResponseDto<List<DepartmanHizmetBinasiDto>>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDropdownByHizmetBinasiAsync hatası. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<DepartmanHizmetBinasiDto>>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<int>> GetDepartmanHizmetBinasiIdAsync(int departmanId, int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<int>>($"{BaseUrl}/find/{departmanId}/{hizmetBinasiId}");
                return response ?? ApiResponseDto<int>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDepartmanHizmetBinasiIdAsync hatası. DepartmanId: {DepartmanId}, HizmetBinasiId: {HizmetBinasiId}", 
                    departmanId, hizmetBinasiId);
                return ApiResponseDto<int>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> CreateAsync(DepartmanHizmetBinasiCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanHizmetBinasiResponseDto>>()
                    ?? ApiResponseDto<DepartmanHizmetBinasiResponseDto>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync hatası");
                return ApiResponseDto<DepartmanHizmetBinasiResponseDto>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<DepartmanHizmetBinasiResponseDto>> UpdateAsync(int id, DepartmanHizmetBinasiUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanHizmetBinasiResponseDto>>()
                    ?? ApiResponseDto<DepartmanHizmetBinasiResponseDto>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync hatası. Id: {Id}", id);
                return ApiResponseDto<DepartmanHizmetBinasiResponseDto>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>()
                    ?? ApiResponseDto<bool>.ErrorResult("Veri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync hatası. Id: {Id}", id);
                return ApiResponseDto<bool>.ErrorResult("Bir hata oluştu", ex.Message);
            }
        }
    }
}
