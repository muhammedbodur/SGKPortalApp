using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
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

        public async Task<ServiceResult<List<KanalPersonelResponseDto>>> GetPersonellerByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalpersonel/departman-hizmet-binasi/{departmanHizmetBinasiId}");

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
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalPersonelResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Personel ataması oluşturulamadı.";
                    _logger.LogError("CreateAsync failed: {Error}", errorMessage);
                    return ServiceResult<KanalPersonelResponseDto>.Fail(errorMessage);
                }

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
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalPersonelResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Personel ataması güncellenemedi.";
                    _logger.LogError("UpdateAsync failed: {Error}", errorMessage);
                    return ServiceResult<KanalPersonelResponseDto>.Fail(errorMessage);
                }

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
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Personel ataması silinemedi.";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

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

        public async Task<ServiceResult<List<PersonelAtamaMatrixDto>>> GetPersonelAtamaMatrixAsync(int departmanHizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalpersonel/matrix/{departmanHizmetBinasiId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetPersonelAtamaMatrixAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelAtamaMatrixDto>>.Fail("Personel atama matrix alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelAtamaMatrixDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelAtamaMatrixDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelAtamaMatrixDto>>.Fail(
                    apiResponse?.Message ?? "Personel atama matrix alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPersonelAtamaMatrixAsync Exception");
                return ServiceResult<List<PersonelAtamaMatrixDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
