using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Auth
{
    public class LoginLogoutLogApiService : ILoginLogoutLogApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LoginLogoutLogApiService> _logger;

        public LoginLogoutLogApiService(
            HttpClient httpClient,
            ILogger<LoginLogoutLogApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<LoginLogoutLogPagedResultDto?> GetLogsAsync(LoginLogoutLogFilterDto filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("loginlogoutlog/logs", filter);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetLogsAsync failed: {Error}", errorContent);
                    return null;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<LoginLogoutLogPagedResultDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }

                _logger.LogWarning("GetLogsAsync returned no data: {Message}", apiResponse?.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLogsAsync Exception");
                return null;
            }
        }

        public async Task<LoginLogoutLogResponseDto?> GetLogByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"loginlogoutlog/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetLogByIdAsync failed: {Error}", errorContent);
                    return null;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<LoginLogoutLogResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }

                _logger.LogWarning("GetLogByIdAsync returned no data: {Message}", apiResponse?.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLogByIdAsync Exception for ID: {Id}", id);
                return null;
            }
        }

        public async Task<int> GetActiveSessionCountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("loginlogoutlog/active-session-count");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveSessionCountAsync failed: {Error}", errorContent);
                    return 0;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();

                if (apiResponse?.Success == true)
                {
                    return apiResponse.Data;
                }

                _logger.LogWarning("GetActiveSessionCountAsync returned no data: {Message}", apiResponse?.Message);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveSessionCountAsync Exception");
                return 0;
            }
        }

        public async Task<int> GetTodayLoginCountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("loginlogoutlog/today-login-count");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetTodayLoginCountAsync failed: {Error}", errorContent);
                    return 0;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();

                if (apiResponse?.Success == true)
                {
                    return apiResponse.Data;
                }

                _logger.LogWarning("GetTodayLoginCountAsync returned no data: {Message}", apiResponse?.Message);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTodayLoginCountAsync Exception");
                return 0;
            }
        }

        public async Task<bool> IsSessionStillValidAsync(string sessionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"loginlogoutlog/is-session-valid/{sessionId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("IsSessionStillValidAsync failed for SessionID: {SessionId}", sessionId);
                    return false; // Hata durumunda güvenli taraf: session geçersiz say
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return apiResponse.Data;
                }

                _logger.LogWarning("IsSessionStillValidAsync returned no data for SessionID: {SessionId}", sessionId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsSessionStillValidAsync Exception for SessionID: {SessionId}", sessionId);
                return false; // Hata durumunda güvenli taraf: session geçersiz say
            }
        }
    }
}
