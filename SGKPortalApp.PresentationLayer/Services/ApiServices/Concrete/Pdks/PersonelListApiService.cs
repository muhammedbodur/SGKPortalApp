using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Pdks
{
    public class PersonelListApiService : IPersonelListApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PersonelListApiService> _logger;

        public PersonelListApiService(HttpClient httpClient, ILogger<PersonelListApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<DepartmanDto>>> GetDepartmanListeAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("departman/liste");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetDepartmanListeAsync failed: {Error}", errorContent);
                    return ServiceResult<List<DepartmanDto>>.Fail("Yetkili departman listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DepartmanDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<DepartmanDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<DepartmanDto>>.Fail(
                    apiResponse?.Message ?? "Yetkili departman listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDepartmanListeAsync Exception");
                return ServiceResult<List<DepartmanDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<ServisDto>>> GetServisListeAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("servis/liste");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetServisListeAsync failed: {Error}", errorContent);
                    return ServiceResult<List<ServisDto>>.Fail("Yetkili servis listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ServisDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<ServisDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<ServisDto>>.Fail(
                    apiResponse?.Message ?? "Yetkili servis listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetServisListeAsync Exception");
                return ServiceResult<List<ServisDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelListResponseDto>>> GetPersonelListeAsync(object request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("personel-list/liste", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetPersonelListeAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelListResponseDto>>.Fail("Personel listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelListResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelListResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelListResponseDto>>.Fail(
                    apiResponse?.Message ?? "Personel listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPersonelListeAsync Exception");
                return ServiceResult<List<PersonelListResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

    }
}
