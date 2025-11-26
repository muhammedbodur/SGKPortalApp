using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.SignalR;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.SignalR
{
    /// <summary>
    /// Hub Connection API Service (Layered Architecture - HttpClient kullanır)
    /// Presentation Layer → API Layer → Business Layer → Data Access Layer
    /// </summary>
    public class HubConnectionApiService : IHubConnectionApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HubConnectionApiService> _logger;

        public HubConnectionApiService(IHttpClientFactory httpClientFactory, ILogger<HubConnectionApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<bool> CreateOrUpdateUserConnectionAsync(string connectionId, string tcKimlikNo)
        {
            try
            {
                var request = new HubConnectionRequestDto
                {
                    ConnectionId = connectionId,
                    TcKimlikNo = tcKimlikNo
                };

                var response = await _httpClient.PostAsJsonAsync("api/hub-connections/connect", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateOrUpdateUserConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> DisconnectAsync(string connectionId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/hub-connections/{connectionId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DisconnectAsync hatası");
                return false;
            }
        }

        public async Task<List<HubConnectionResponseDto>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<HubConnectionResponseDto>>(
                    $"api/hub-connections/active/{tcKimlikNo}");
                
                return response ?? new List<HubConnectionResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveConnectionsByTcKimlikNoAsync hatası");
                return new List<HubConnectionResponseDto>();
            }
        }

        public async Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, string tcKimlikNo)
        {
            try
            {
                var request = new BankoConnectionRequestDto
                {
                    BankoId = bankoId,
                    ConnectionId = connectionId,
                    TcKimlikNo = tcKimlikNo
                };

                var response = await _httpClient.PostAsJsonAsync("api/hub-connections/banko/register", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterBankoConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> DeactivateBankoConnectionAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/hub-connections/banko/deactivate", tcKimlikNo);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateBankoConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, string tcKimlikNo)
        {
            try
            {
                var request = new TvConnectionRequestDto
                {
                    TvId = tvId,
                    ConnectionId = connectionId,
                    TcKimlikNo = tcKimlikNo
                };

                var response = await _httpClient.PostAsJsonAsync("api/hub-connections/tv/register", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterTvConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> IsBankoInUseAsync(int bankoId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<bool>(
                    $"api/hub-connections/banko/{bankoId}/in-use");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsBankoInUseAsync hatası");
                return false;
            }
        }

        public async Task<HubBankoConnectionResponseDto?> GetPersonelActiveBankoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<HubBankoConnectionResponseDto>(
                    $"api/hub-connections/personel/{tcKimlikNo}/active-banko");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPersonelActiveBankoAsync hatası");
                return null;
            }
        }

        public async Task<bool> IsTvInUseByTvUserAsync(int tvId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<bool>(
                    $"api/hub-connections/tv/{tvId}/in-use");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsTvInUseByTvUserAsync hatası");
                return false;
            }
        }

        public async Task<bool> UpdateConnectionTypeAsync(string connectionId, string connectionType)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"api/hub-connections/{connectionId}/type", 
                    connectionType);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateConnectionTypeAsync hatası");
                return false;
            }
        }

        public async Task<bool> SetConnectionStatusAsync(string connectionId, string status)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"api/hub-connections/{connectionId}/status", 
                    status);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetConnectionStatusAsync hatası");
                return false;
            }
        }

        public async Task<HubConnectionResponseDto?> GetByConnectionIdAsync(string connectionId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<HubConnectionResponseDto>(
                    $"api/hub-connections/{connectionId}");
                return response;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // 404 normal - bağlantı bulunamadı (zaten silinmiş olabilir)
                _logger.LogDebug("Connection bulunamadı: {ConnectionId}", connectionId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByConnectionIdAsync hatası");
                return null;
            }
        }

        public async Task<bool> CreateBankoConnectionAsync(int hubConnectionId, int bankoId, string tcKimlikNo)
        {
            try
            {
                var request = new
                {
                    HubConnectionId = hubConnectionId,
                    BankoId = bankoId,
                    TcKimlikNo = tcKimlikNo
                };

                var response = await _httpClient.PostAsJsonAsync("api/hub-connections/banko", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateBankoConnectionAsync hatası");
                return false;
            }
        }

        public async Task<bool> DeactivateBankoConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/hub-connections/banko/{hubConnectionId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateBankoConnectionByHubConnectionIdAsync hatası");
                return false;
            }
        }

        public async Task<List<HubConnectionResponseDto>> GetNonBankoConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<HubConnectionResponseDto>>(
                    $"api/hub-connections/non-banko/{tcKimlikNo}");
                return response ?? new List<HubConnectionResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetNonBankoConnectionsByTcKimlikNoAsync hatası");
                return new List<HubConnectionResponseDto>();
            }
        }

        public async Task<HubBankoConnectionResponseDto?> GetBankoConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<HubBankoConnectionResponseDto>(
                    $"api/hub-connections/banko-connection/{hubConnectionId}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBankoConnectionByHubConnectionIdAsync hatası");
                return null;
            }
        }

        public async Task<HubTvConnectionResponseDto?> GetTvConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<HubTvConnectionResponseDto>(
                    $"api/hub-connections/tv-connection/{hubConnectionId}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTvConnectionByHubConnectionIdAsync hatası");
                return null;
            }
        }

        public async Task<UserResponseDto?> GetBankoActivePersonelAsync(int bankoId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<UserResponseDto>(
                    $"api/hub-connections/banko/{bankoId}/active-personel");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBankoActivePersonelAsync hatası");
                return null;
            }
        }
    }
}

