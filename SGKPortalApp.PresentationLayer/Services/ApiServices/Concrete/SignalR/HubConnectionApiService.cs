using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
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

        public async Task<List<HubConnection>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<HubConnection>>(
                    $"api/hub-connections/active/{tcKimlikNo}");
                
                return response ?? new List<HubConnection>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveConnectionsByTcKimlikNoAsync hatası");
                return new List<HubConnection>();
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

        public async Task<HubBankoConnection?> GetPersonelActiveBankoAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<HubBankoConnection>(
                    $"api/hub-connections/personel/{tcKimlikNo}/active-banko");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPersonelActiveBankoAsync hatası");
                return null;
            }
        }

        public async Task<User?> GetBankoActivePersonelAsync(int bankoId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<User>(
                    $"api/hub-connections/banko/{bankoId}/active-personel");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBankoActivePersonelAsync hatası");
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

        public async Task<HubConnection?> GetByConnectionIdAsync(string connectionId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<HubConnection>(
                    $"api/hub-connections/{connectionId}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByConnectionIdAsync hatası");
                return null;
            }
        }
    }
}
