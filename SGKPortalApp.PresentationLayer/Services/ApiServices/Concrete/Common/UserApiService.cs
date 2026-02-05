using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Account;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserApiService> _logger;

        public UserApiService(HttpClient httpClient, ILogger<UserApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<UserResponseDto>> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"user/{tcKimlikNo}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByTcKimlikNoAsync failed: {Error}", errorContent);
                    return ServiceResult<UserResponseDto>.Fail("Kullanıcı bilgisi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<UserResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<UserResponseDto>.Fail(
                    apiResponse?.Message ?? "Kullanıcı bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByTcKimlikNoAsync exception");
                return ServiceResult<UserResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ChangePasswordAsync(string tcKimlikNo, ChangePasswordRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"user/{tcKimlikNo}/change-password", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ChangePasswordAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Şifre değiştirilemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Şifre değiştirilemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangePasswordAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ActivateBankoModeAsync(string tcKimlikNo, int bankoId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"user/{tcKimlikNo}/banko-mode/activate", bankoId);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ActivateBankoModeAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Banko modu aktif edilemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Banko modu başarıyla aktif edildi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Banko modu aktif edilemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActivateBankoModeAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateBankoModeAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.PostAsync($"user/{tcKimlikNo}/banko-mode/deactivate", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeactivateBankoModeAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Banko modu deaktif edilemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Banko modu başarıyla deaktif edildi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Banko modu deaktif edilemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateBankoModeAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> IsBankoModeActiveAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"user/{tcKimlikNo}/banko-mode/is-active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("IsBankoModeActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Banko modu durumu kontrol edilemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Banko modu durumu kontrol edilemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsBankoModeActiveAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int?>> GetActiveBankoIdAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"user/{tcKimlikNo}/banko-mode/active-banko-id");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveBankoIdAsync failed: {Error}", errorContent);
                    return ServiceResult<int?>.Fail("Aktif banko ID alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<int?>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<int?>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<int?>.Fail(
                    apiResponse?.Message ?? "Aktif banko ID alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveBankoIdAsync exception");
                return ServiceResult<int?>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
