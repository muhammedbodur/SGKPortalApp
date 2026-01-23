using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class BankoApiService : IBankoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BankoApiService> _logger;

        public BankoApiService(HttpClient httpClient, ILogger<BankoApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════
        // QUERY OPERATIONS
        // ═══════════════════════════════════════════════════════

        public async Task<ServiceResult<List<BankoResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("banko");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<BankoResponseDto>>.Fail("Banko listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<BankoResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<BankoResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<BankoResponseDto>>.Fail(
                    apiResponse?.Message ?? "Banko listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<BankoResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BankoResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"banko/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<BankoResponseDto>.Fail("Banko bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<BankoResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<BankoResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<BankoResponseDto>.Fail(
                    apiResponse?.Message ?? "Banko bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<BankoResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<BankoResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"banko/departman-hizmet-binasi/{hizmetBinasiId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByHizmetBinasiAsync failed: {Error}", errorContent);
                    return ServiceResult<List<BankoResponseDto>>.Fail("Bankolar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<BankoResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<BankoResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<BankoResponseDto>>.Fail(
                    apiResponse?.Message ?? "Bankolar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByHizmetBinasiAsync Exception");
                return ServiceResult<List<BankoResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<BankoKatGrupluResponseDto>>> GetGroupedByKatAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"banko/departman-hizmet-binasi/{hizmetBinasiId}/grouped");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetGroupedByKatAsync failed: {Error}", errorContent);
                    return ServiceResult<List<BankoKatGrupluResponseDto>>.Fail("Kat gruplu bankolar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<BankoKatGrupluResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<BankoKatGrupluResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<BankoKatGrupluResponseDto>>.Fail(
                    apiResponse?.Message ?? "Kat gruplu bankolar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetGroupedByKatAsync Exception");
                return ServiceResult<List<BankoKatGrupluResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<BankoResponseDto>>> GetAvailableBankosAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"banko/departman-hizmet-binasi/{hizmetBinasiId}/bos");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAvailableBankosAsync failed: {Error}", errorContent);
                    return ServiceResult<List<BankoResponseDto>>.Fail("Boş bankolar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<BankoResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<BankoResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<BankoResponseDto>>.Fail(
                    apiResponse?.Message ?? "Boş bankolar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAvailableBankosAsync Exception");
                return ServiceResult<List<BankoResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<BankoResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("banko/aktif");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<BankoResponseDto>>.Fail("Aktif bankolar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<BankoResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<BankoResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<BankoResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif bankolar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<BankoResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BankoResponseDto>> GetPersonelCurrentBankoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"banko/personel/{tcKimlikNo}/banko");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetPersonelCurrentBankoAsync failed: {Error}", errorContent);
                    return ServiceResult<BankoResponseDto>.Fail("Personelin bankosu bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<BankoResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<BankoResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<BankoResponseDto>.Fail(
                    apiResponse?.Message ?? "Personelin bankosu bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPersonelCurrentBankoAsync Exception");
                return ServiceResult<BankoResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════
        // CRUD OPERATIONS
        // ═══════════════════════════════════════════════════════

        public async Task<ServiceResult<BankoResponseDto>> CreateAsync(BankoCreateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("banko", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<BankoResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Banko eklenemedi.";
                    _logger.LogError("CreateAsync failed: {Error}", errorMessage);
                    return ServiceResult<BankoResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<BankoResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Banko başarıyla eklendi"
                    );
                }

                return ServiceResult<BankoResponseDto>.Fail(
                    apiResponse?.Message ?? "Banko eklenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<BankoResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BankoResponseDto>> UpdateAsync(int id, BankoUpdateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"banko/{id}", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<BankoResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Banko güncellenemedi.";
                    _logger.LogError("UpdateAsync failed: {Error}", errorMessage);
                    return ServiceResult<BankoResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<BankoResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Banko başarıyla güncellendi"
                    );
                }

                return ServiceResult<BankoResponseDto>.Fail(
                    apiResponse?.Message ?? "Banko güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<BankoResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"banko/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Banko silinemedi.";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Banko başarıyla silindi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Banko silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL ATAMA OPERATIONS
        // ═══════════════════════════════════════════════════════

        public async Task<ServiceResult<bool>> PersonelAtaAsync(BankoPersonelAtaDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("banko/ata", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Personel atanamadı.";
                    _logger.LogError("PersonelAtaAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Personel başarıyla atandı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Personel atanamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PersonelAtaAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> PersonelCikarAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"banko/personel/{tcKimlikNo}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Personel çıkarılamadı.";
                    _logger.LogError("PersonelCikarAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Personel başarıyla çıkarıldı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Personel çıkarılamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PersonelCikarAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> BankoBoşaltAsync(int bankoId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"banko/{bankoId}/bosalt");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Banko boşaltılamadı.";
                    _logger.LogError("BankoBoşaltAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Banko başarıyla boşaltıldı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Banko boşaltılamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BankoBoşaltAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════
        // AKTIFLIK OPERATIONS
        // ═══════════════════════════════════════════════════════

        public async Task<ServiceResult<bool>> ToggleAktiflikAsync(int bankoId)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"banko/{bankoId}/toggle-aktiflik", null);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Aktiflik durumu değiştirilemedi.";
                    _logger.LogError("ToggleAktiflikAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Aktiflik durumu başarıyla değiştirildi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Aktiflik durumu değiştirilemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToggleAktiflikAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
