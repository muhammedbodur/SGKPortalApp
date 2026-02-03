using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class PersonelYetkiApiService : IPersonelYetkiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PersonelYetkiApiService> _logger;

        public PersonelYetkiApiService(HttpClient httpClient, ILogger<PersonelYetkiApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<PersonelYetkiResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personelyetki/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<PersonelYetkiResponseDto>.Fail("Personel yetkisi bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PersonelYetkiResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PersonelYetkiResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<PersonelYetkiResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel yetkisi bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<PersonelYetkiResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelYetkiResponseDto>>> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personelyetki/by-tc/{tcKimlikNo}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByTcKimlikNoAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelYetkiResponseDto>>.Fail("Personel yetkileri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelYetkiResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelYetkiResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelYetkiResponseDto>>.Fail(
                    apiResponse?.Message ?? "Personel yetkileri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByTcKimlikNoAsync Exception");
                return ServiceResult<List<PersonelYetkiResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelYetkiResponseDto>>> GetByYetkiIdAsync(int yetkiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personelyetki/by-yetki/{yetkiId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByYetkiIdAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelYetkiResponseDto>>.Fail("Yetkiye bağlı personel yetkileri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelYetkiResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelYetkiResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelYetkiResponseDto>>.Fail(
                    apiResponse?.Message ?? "Yetkiye bağlı personel yetkileri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByYetkiIdAsync Exception");
                return ServiceResult<List<PersonelYetkiResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PersonelYetkiResponseDto>> CreateAsync(PersonelYetkiCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("personelyetki", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<PersonelYetkiResponseDto>.Fail("Personel yetkisi oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PersonelYetkiResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PersonelYetkiResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<PersonelYetkiResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel yetkisi oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<PersonelYetkiResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PersonelYetkiResponseDto>> UpdateAsync(int id, PersonelYetkiUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"personelyetki/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<PersonelYetkiResponseDto>.Fail("Personel yetkisi güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PersonelYetkiResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PersonelYetkiResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<PersonelYetkiResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel yetkisi güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<PersonelYetkiResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"personelyetki/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Personel yetkisi silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(true, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Personel yetkisi silinemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Dictionary<string, int>>> GetUserPermissionsWithDefaultsAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personelyetki/with-defaults/{tcKimlikNo}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetUserPermissionsWithDefaultsAsync failed: {Error}", errorContent);
                    return ServiceResult<Dictionary<string, int>>.Fail("Yetkiler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<Dictionary<string, int>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<Dictionary<string, int>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<Dictionary<string, int>>.Fail(
                    apiResponse?.Message ?? "Yetkiler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserPermissionsWithDefaultsAsync Exception");
                return ServiceResult<Dictionary<string, int>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
