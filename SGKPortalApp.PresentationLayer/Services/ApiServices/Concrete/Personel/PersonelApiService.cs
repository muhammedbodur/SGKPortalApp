using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using System.Net.Http.Json;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class PersonelApiService : IPersonelApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PersonelApiService> _logger;

        public PersonelApiService(HttpClient httpClient, ILogger<PersonelApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<PersonelResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("personel");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelResponseDto>>.Fail("Personel listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelResponseDto>>.Fail(
                    apiResponse?.Message ?? "Personel listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<PersonelResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PersonelResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personel/{tcKimlikNo}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByTcKimlikNoAsync failed: {Error}", errorContent);
                    return ServiceResult<PersonelResponseDto>.Fail("Personel bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PersonelResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PersonelResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<PersonelResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByTcKimlikNoAsync Exception for TC: {TcKimlikNo}", tcKimlikNo);
                return ServiceResult<PersonelResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PersonelResponseDto>> UpdateAsync(string tcKimlikNo, PersonelUpdateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"personel/{tcKimlikNo}", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<PersonelResponseDto>.Fail("Personel güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PersonelResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Personel başarıyla güncellendi!";

                    return ServiceResult<PersonelResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                return ServiceResult<PersonelResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<PersonelResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"personel/{tcKimlikNo}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Personel silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Personel başarıyla silindi!";

                    return ServiceResult<bool>.Ok(true, successMessage);
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Personel silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PersonelResponseDto>> CreateCompleteAsync(PersonelCompleteRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("personel/complete", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateCompleteAsync failed: {Error}", errorContent);
                    return ServiceResult<PersonelResponseDto>.Fail("Personel toplu kaydedilemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PersonelResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PersonelResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Personel başarıyla kaydedildi"
                    );
                }

                return ServiceResult<PersonelResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel kaydedilemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateCompleteAsync Exception");
                return ServiceResult<PersonelResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PersonelResponseDto>> UpdateCompleteAsync(string tcKimlikNo, PersonelCompleteRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"personel/{tcKimlikNo}/complete", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateCompleteAsync failed: {Error}", errorContent);
                    return ServiceResult<PersonelResponseDto>.Fail("Personel toplu güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PersonelResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PersonelResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Personel başarıyla güncellendi"
                    );
                }

                return ServiceResult<PersonelResponseDto>.Fail(
                    apiResponse?.Message ?? "Personel güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateCompleteAsync Exception");
                return ServiceResult<PersonelResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("personel/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelResponseDto>>.Fail("Aktif personeller alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif personeller alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<PersonelResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResponseDto<PersonelListResponseDto>>> GetPagedAsync(PersonelFilterRequestDto filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("personel/paged", filter);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetPagedAsync failed: {Error}", errorContent);
                    return ServiceResult<PagedResponseDto<PersonelListResponseDto>>.Fail("Sayfalı personel listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResponseDto<PersonelListResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PagedResponseDto<PersonelListResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<PagedResponseDto<PersonelListResponseDto>>.Fail(
                    apiResponse?.Message ?? "Personel listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPagedAsync Exception");
                return ServiceResult<PagedResponseDto<PersonelListResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelResponseDto>>> GetByDepartmanAsync(int departmanId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personel/departman/{departmanId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByDepartmanAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelResponseDto>>.Fail("Departman personelleri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelResponseDto>>.Fail(
                    apiResponse?.Message ?? "Departman personelleri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByDepartmanAsync Exception");
                return ServiceResult<List<PersonelResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelResponseDto>>> GetByServisAsync(int servisId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personel/servis/{servisId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByServisAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelResponseDto>>.Fail("Servis personelleri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelResponseDto>>.Fail(
                    apiResponse?.Message ?? "Servis personelleri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByServisAsync Exception");
                return ServiceResult<List<PersonelResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelResponseDto>>> GetPersonellerByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personel/hizmet-binasi/{hizmetBinasiId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByHizmetBinasiIdAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelResponseDto>>.Fail("Hizmet binası personelleri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<PersonelResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<PersonelResponseDto>>.Fail(
                    apiResponse?.Message ?? "Hizmet binası personelleri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByHizmetBinasiIdAsync Exception");
                return ServiceResult<List<PersonelResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PdksCardSendResultDto>> SendCardToAllDevicesAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.PostAsync($"personel/{tcKimlikNo}/pdks/send-to-all-devices", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("SendCardToAllDevicesAsync failed: {Error}", errorContent);
                    return ServiceResult<PdksCardSendResultDto>.Fail("Kart bilgisi gönderilemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PdksCardSendResultDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PdksCardSendResultDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<PdksCardSendResultDto>.Fail(
                    apiResponse?.Message ?? "Kart bilgisi gönderilemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendCardToAllDevicesAsync Exception");
                return ServiceResult<PdksCardSendResultDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PdksCardSendResultDto>> DeleteCardFromAllDevicesAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"personel/{tcKimlikNo}/pdks/delete-from-all-devices");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteCardFromAllDevicesAsync failed: {Error}", errorContent);
                    return ServiceResult<PdksCardSendResultDto>.Fail("Kart bilgisi silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PdksCardSendResultDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PdksCardSendResultDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<PdksCardSendResultDto>.Fail(
                    apiResponse?.Message ?? "Kart bilgisi silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteCardFromAllDevicesAsync Exception");
                return ServiceResult<PdksCardSendResultDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelListResponseDto>>> SearchAsync(string term, int limit = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"personel/search?term={Uri.EscapeDataString(term)}&limit={limit}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("SearchAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelListResponseDto>>.Fail("Arama yapılamadı.");
                }

                var searchResults = await response.Content.ReadFromJsonAsync<List<PersonelListResponseDto>>();

                if (searchResults != null)
                {
                    return ServiceResult<List<PersonelListResponseDto>>.Ok(searchResults, "Arama başarılı");
                }

                return ServiceResult<List<PersonelListResponseDto>>.Fail("Arama sonucu alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchAsync Exception");
                return ServiceResult<List<PersonelListResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}