using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Pdks
{
    public class PersonelMesaiApiService : IPersonelMesaiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PersonelMesaiApiService> _logger;

        public PersonelMesaiApiService(HttpClient httpClient, ILogger<PersonelMesaiApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<PersonelMesaiBaslikDto>> GetBaslikAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personel-mesai/baslik/{tcKimlikNo}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetBaslikAsync failed: {Error}", errorContent);
                    return ServiceResult<PersonelMesaiBaslikDto>.Fail("Personel başlık bilgisi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PersonelMesaiBaslikDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PersonelMesaiBaslikDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<PersonelMesaiBaslikDto>.Fail(
                    apiResponse?.Message ?? "Personel başlık bilgisi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBaslikAsync Exception");
                return ServiceResult<PersonelMesaiBaslikDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelMesaiListResponseDto>>> GetListeAsync(PersonelMesaiFilterRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("personel-mesai/liste", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetListeAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelMesaiListResponseDto>>.Fail("Mesai listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelMesaiListResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelMesaiListResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelMesaiListResponseDto>>.Fail(
                    apiResponse?.Message ?? "Mesai listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetListeAsync Exception");
                return ServiceResult<List<PersonelMesaiListResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
