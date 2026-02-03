using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class KanalIslemApiService : IKanalIslemApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KanalIslemApiService> _logger;

        public KanalIslemApiService(HttpClient httpClient, ILogger<KanalIslemApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<KanalIslemResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("kanalislem");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalIslemResponseDto>>.Fail("Kanal işlem listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalIslemResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalIslemResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalIslemResponseDto>>.Fail(
                    apiResponse?.Message ?? "Kanal işlem listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<KanalIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalIslemResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalislem/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<KanalIslemResponseDto>.Fail("Kanal işlem bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalIslemResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalIslemResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<KanalIslemResponseDto>.Fail(
                    apiResponse?.Message ?? "Kanal işlem bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<KanalIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalIslemResponseDto>> CreateAsync(KanalIslemCreateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("kanalislem", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalIslemResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Kanal işlem oluşturulamadı.";
                    _logger.LogError("CreateAsync failed: {Error}", errorMessage);
                    return ServiceResult<KanalIslemResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalIslemResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Kanal işlem başarıyla oluşturuldu"
                    );
                }

                return ServiceResult<KanalIslemResponseDto>.Fail(
                    apiResponse?.Message ?? "Kanal işlem oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<KanalIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalIslemResponseDto>> UpdateAsync(int id, KanalIslemUpdateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"kanalislem/{id}", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalIslemResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Kanal işlem güncellenemedi.";
                    _logger.LogError("UpdateAsync failed: {Error}", errorMessage);
                    return ServiceResult<KanalIslemResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalIslemResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Kanal işlem başarıyla güncellendi"
                    );
                }

                return ServiceResult<KanalIslemResponseDto>.Fail(
                    apiResponse?.Message ?? "Kanal işlem güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<KanalIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"kanalislem/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Kanal işlem silinemedi.";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Kanal işlem başarıyla silindi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Kanal işlem silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalIslemResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("kanalislem/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalIslemResponseDto>>.Fail("Aktif kanal işlemleri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalIslemResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalIslemResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalIslemResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif kanal işlemleri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<KanalIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalIslemResponseDto>>> GetByKanalIdAsync(int kanalId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalislem/kanal/{kanalId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByKanalIdAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalIslemResponseDto>>.Fail("Kanal işlemleri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalIslemResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalIslemResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalIslemResponseDto>>.Fail(
                    apiResponse?.Message ?? "Kanal işlemleri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByKanalIdAsync Exception");
                return ServiceResult<List<KanalIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalIslemResponseDto>>> GetByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalislem/departman-hizmet-binasi/{departmanHizmetBinasiId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByHizmetBinasiIdAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalIslemResponseDto>>.Fail("Hizmet binası kanal işlemleri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalIslemResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalIslemResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalIslemResponseDto>>.Fail(
                    apiResponse?.Message ?? "Hizmet binası kanal işlemleri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByHizmetBinasiIdAsync Exception");
                return ServiceResult<List<KanalIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
