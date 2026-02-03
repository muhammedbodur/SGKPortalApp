using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class IlceApiService : IIlceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IlceApiService> _logger;

        public IlceApiService(HttpClient httpClient, ILogger<IlceApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<IlceResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("ilce");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<IlceResponseDto>>.Fail("İlçeler listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<IlceResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<IlceResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<IlceResponseDto>>.Fail(
                    apiResponse?.Message ?? "İlçeler listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync exception");
                return ServiceResult<List<IlceResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IlceResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"ilce/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<IlceResponseDto>.Fail("İlçe bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IlceResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IlceResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<IlceResponseDto>.Fail(
                    apiResponse?.Message ?? "İlçe bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync exception");
                return ServiceResult<IlceResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IlceResponseDto>> CreateAsync(IlceCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("ilce", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<IlceResponseDto>.Fail("İlçe oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IlceResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IlceResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<IlceResponseDto>.Fail(
                    apiResponse?.Message ?? "İlçe oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync exception");
                return ServiceResult<IlceResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IlceResponseDto>> UpdateAsync(int id, IlceUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"ilce/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<IlceResponseDto>.Fail("İlçe güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IlceResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IlceResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<IlceResponseDto>.Fail(
                    apiResponse?.Message ?? "İlçe güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync exception");
                return ServiceResult<IlceResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"ilce/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("İlçe silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "İlçe silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<IlceResponseDto>>> GetByIlAsync(int ilId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"ilce/by-il/{ilId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIlAsync failed: {Error}", errorContent);
                    return ServiceResult<List<IlceResponseDto>>.Fail("İl bazlı ilçeler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<IlceResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<IlceResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<IlceResponseDto>>.Fail(
                    apiResponse?.Message ?? "İl bazlı ilçeler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIlAsync exception");
                return ServiceResult<List<IlceResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<(int Id, string Ad)>>> GetDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("ilce/dropdown");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetDropdownAsync failed: {Error}", errorContent);
                    return ServiceResult<List<(int Id, string Ad)>>.Fail("Dropdown listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<(int Id, string Ad)>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<(int Id, string Ad)>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<(int Id, string Ad)>>.Fail(
                    apiResponse?.Message ?? "Dropdown listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDropdownAsync exception");
                return ServiceResult<List<(int Id, string Ad)>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<(int Id, string Ad)>>> GetByIlDropdownAsync(int ilId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"ilce/dropdown/by-il/{ilId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIlDropdownAsync failed: {Error}", errorContent);
                    return ServiceResult<List<(int Id, string Ad)>>.Fail("İl bazlı dropdown listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<(int Id, string Ad)>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<(int Id, string Ad)>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<(int Id, string Ad)>>.Fail(
                    apiResponse?.Message ?? "İl bazlı dropdown listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIlDropdownAsync exception");
                return ServiceResult<List<(int Id, string Ad)>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
