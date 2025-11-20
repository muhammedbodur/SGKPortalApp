using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class KanalAltIslemApiService : IKanalAltIslemApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KanalAltIslemApiService> _logger;

        public KanalAltIslemApiService(HttpClient httpClient, ILogger<KanalAltIslemApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<KanalAltIslemResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("KanalAltIslem");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalAltIslemResponseDto>>.Fail("Kanal alt işlem listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalAltIslemResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalAltIslemResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalAltIslemResponseDto>>.Fail(
                    apiResponse?.Message ?? "Kanal alt işlem listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<KanalAltIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalAltIslemResponseDto>> GetByIdWithDetailsAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalaltislem/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdWithDetailsAsync failed: {Error}", errorContent);
                    return ServiceResult<KanalAltIslemResponseDto>.Fail("Kanal alt işlem bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalAltIslemResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalAltIslemResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<KanalAltIslemResponseDto>.Fail(
                    apiResponse?.Message ?? "Kanal alt işlem bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdWithDetailsAsync Exception");
                return ServiceResult<KanalAltIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalAltIslemResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalaltislem/hizmet-binasi/{hizmetBinasiId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByHizmetBinasiIdAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalAltIslemResponseDto>>.Fail("Kanal alt işlemler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalAltIslemResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalAltIslemResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalAltIslemResponseDto>>.Fail(
                    apiResponse?.Message ?? "Kanal alt işlemler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByHizmetBinasiIdAsync Exception");
                return ServiceResult<List<KanalAltIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalAltIslemResponseDto>>> GetByKanalIslemIdAsync(int kanalIslemId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalaltislem/kanal-islem/{kanalIslemId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByKanalIslemIdAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalAltIslemResponseDto>>.Fail("Kanal alt işlemler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalAltIslemResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalAltIslemResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalAltIslemResponseDto>>.Fail(
                    apiResponse?.Message ?? "Kanal alt işlemler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByKanalIslemIdAsync Exception");
                return ServiceResult<List<KanalAltIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Dictionary<int, int>>> GetPersonelSayilariAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalaltislem/hizmet-binasi/{hizmetBinasiId}/personel-sayilari");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetPersonelSayilariAsync failed: {Error}", errorContent);
                    return ServiceResult<Dictionary<int, int>>.Fail("Personel sayıları alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<Dictionary<int, int>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<Dictionary<int, int>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<Dictionary<int, int>>.Fail(
                    apiResponse?.Message ?? "Personel sayıları alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPersonelSayilariAsync Exception");
                return ServiceResult<Dictionary<int, int>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalAltIslemResponseDto>>> GetEslestirmeYapilmamisAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalaltislem/hizmet-binasi/{hizmetBinasiId}/eslestirme-yapilmamis");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetEslestirmeYapilmamisAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalAltIslemResponseDto>>.Fail("Eşleştirilmemiş işlemler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalAltIslemResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalAltIslemResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalAltIslemResponseDto>>.Fail(
                    apiResponse?.Message ?? "Eşleştirilmemiş işlemler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetEslestirmeYapilmamisAsync Exception");
                return ServiceResult<List<KanalAltIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalAltIslemResponseDto>> CreateAsync(KanalAltIslemCreateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("kanalaltislem", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<KanalAltIslemResponseDto>.Fail("Kanal alt işlem oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalAltIslemResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalAltIslemResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Kanal alt işlem başarıyla oluşturuldu"
                    );
                }

                return ServiceResult<KanalAltIslemResponseDto>.Fail(
                    apiResponse?.Message ?? "Kanal alt işlem oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<KanalAltIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalAltIslemResponseDto>> UpdateAsync(int id, KanalAltIslemUpdateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"kanalaltislem/{id}", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<KanalAltIslemResponseDto>.Fail("Kanal alt işlem güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalAltIslemResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalAltIslemResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Kanal alt işlem başarıyla güncellendi"
                    );
                }

                return ServiceResult<KanalAltIslemResponseDto>.Fail(
                    apiResponse?.Message ?? "Kanal alt işlem güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<KanalAltIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"kanalaltislem/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Kanal alt işlem silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Kanal alt işlem başarıyla silindi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Kanal alt işlem silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
