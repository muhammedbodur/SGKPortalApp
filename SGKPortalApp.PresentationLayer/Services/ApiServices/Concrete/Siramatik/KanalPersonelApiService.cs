using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class KanalPersonelApiService : IKanalPersonelApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KanalPersonelApiService> _logger;

        public KanalPersonelApiService(HttpClient httpClient, ILogger<KanalPersonelApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<KanalPersonelResponseDto>>> GetPersonellerByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalpersonel/hizmet-binasi/{hizmetBinasiId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByHizmetBinasiIdAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalPersonelResponseDto>>.Fail("Personel atamaları alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalPersonelResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalPersonelResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalPersonelResponseDto>>.Fail(
                    apiResponse?.Message ?? "Personel atamaları alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByHizmetBinasiIdAsync Exception");
                return ServiceResult<List<KanalPersonelResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalPersonelResponseDto>>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalpersonel/personel/{tcKimlikNo}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByPersonelTcAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalPersonelResponseDto>>.Fail("Personel atamaları alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalPersonelResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalPersonelResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalPersonelResponseDto>>.Fail(
                    apiResponse?.Message ?? "Personel atamaları alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByPersonelTcAsync Exception");
                return ServiceResult<List<KanalPersonelResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalPersonelResponseDto>>> GetByKanalAltIslemIdAsync(int kanalAltIslemId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalpersonel/kanal-alt-islem/{kanalAltIslemId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByKanalAltIslemIdAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalPersonelResponseDto>>.Fail("Personel atamaları alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalPersonelResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalPersonelResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalPersonelResponseDto>>.Fail(
                    apiResponse?.Message ?? "Personel atamaları alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByKanalAltIslemIdAsync Exception");
                return ServiceResult<List<KanalPersonelResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalPersonelResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalpersonel/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<KanalPersonelResponseDto>.Fail("Personel ataması bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalPersonelResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalPersonelResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<KanalPersonelResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel ataması bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<KanalPersonelResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalPersonelResponseDto>> CreateAsync(KanalPersonelCreateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("kanalpersonel", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<KanalPersonelResponseDto>.Fail("Personel ataması oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalPersonelResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalPersonelResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Personel ataması başarıyla oluşturuldu"
                    );
                }

                return ServiceResult<KanalPersonelResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel ataması oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<KanalPersonelResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalPersonelResponseDto>> UpdateAsync(int id, KanalPersonelUpdateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"kanalpersonel/{id}", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<KanalPersonelResponseDto>.Fail("Personel ataması güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalPersonelResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalPersonelResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Personel ataması başarıyla güncellendi"
                    );
                }

                return ServiceResult<KanalPersonelResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel ataması güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<KanalPersonelResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"kanalpersonel/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Personel ataması silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Personel ataması başarıyla silindi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Personel ataması silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // ⚠️ DEPRECATED METHODS - Kullanılmamaktadır
        // ═══════════════════════════════════════════════════════════════════════════════
        // GetPersonelMatrixAsync() - Kullanılmıyor
        // TogglePersonelUzmanlikAsync() - Kullanılmıyor
    }
}
