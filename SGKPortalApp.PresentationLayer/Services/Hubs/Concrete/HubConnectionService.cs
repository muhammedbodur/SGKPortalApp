using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.SignalR;
using SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces;

namespace SGKPortalApp.PresentationLayer.Services.Hubs.Concrete
{
    /// <summary>
    /// Hub Connection Service (Layered Architecture - API üzerinden çalışır)
    /// 
    /// SignalR Hub → HubConnectionService → HubConnectionApiService → API → Business → Data
    /// </summary>
    public class HubConnectionService : IHubConnectionService
    {
        private readonly IHubConnectionApiService _apiService;
        private readonly ILogger<HubConnectionService> _logger;

        public HubConnectionService(
            IHubConnectionApiService apiService,
            ILogger<HubConnectionService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════
        // CORE CONNECTION MANAGEMENT
        // ═══════════════════════════════════════════════════════

        public async Task<bool> CreateOrUpdateUserConnectionAsync(string connectionId, string tcKimlikNo)
        {
            return await _apiService.CreateOrUpdateUserConnectionAsync(connectionId, tcKimlikNo);
        }

        public async Task<bool> DisconnectAsync(string connectionId)
        {
            return await _apiService.DisconnectAsync(connectionId);
        }

        public async Task<List<HubConnection>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo)
        {
            return await _apiService.GetActiveConnectionsByTcKimlikNoAsync(tcKimlikNo);
        }

        public async Task<HubConnection?> GetByConnectionIdAsync(string connectionId)
        {
            return await _apiService.GetByConnectionIdAsync(connectionId);
        }

        public async Task<bool> UpdateConnectionTypeAsync(string connectionId, string connectionType)
        {
            return await _apiService.UpdateConnectionTypeAsync(connectionId, connectionType);
        }

        public async Task<bool> SetConnectionStatusAsync(string connectionId, string status)
        {
            return await _apiService.SetConnectionStatusAsync(connectionId, status);
        }

        public async Task<bool> RegisterUserConnectionAsync(string connectionId, string tcKimlikNo, string connectionType)
        {
            return await _apiService.CreateOrUpdateUserConnectionAsync(connectionId, tcKimlikNo);
        }

        // ═══════════════════════════════════════════════════════
        // BANKO CONNECTION MANAGEMENT
        // ═══════════════════════════════════════════════════════

        public async Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, string tcKimlikNo)
        {
            return await _apiService.RegisterBankoConnectionAsync(bankoId, connectionId, tcKimlikNo);
        }

        public async Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, ConnectionStatus status)
        {
            _logger.LogWarning("RegisterBankoConnectionAsync ConnectionStatus ile çağrıldı - boş tcKimlikNo kullanılıyor");
            return await _apiService.RegisterBankoConnectionAsync(bankoId, connectionId, "");
        }

        public async Task<bool> UnregisterBankoConnectionAsync(int bankoId, string connectionId)
        {
            _logger.LogWarning("UnregisterBankoConnectionAsync - API endpoint henüz implement edilmedi");
            return await _apiService.DeactivateBankoConnectionAsync("");
        }

        public async Task<bool> DeactivateBankoConnectionAsync(string tcKimlikNo)
        {
            return await _apiService.DeactivateBankoConnectionAsync(tcKimlikNo);
        }

        public async Task<bool> IsBankoInUseAsync(int bankoId)
        {
            return await _apiService.IsBankoInUseAsync(bankoId);
        }

        public async Task<bool> IsBankoConnectedAsync(int bankoId)
        {
            return await _apiService.IsBankoInUseAsync(bankoId);
        }

        public async Task<HubBankoConnection?> GetPersonelActiveBankoAsync(string tcKimlikNo)
        {
            return await _apiService.GetPersonelActiveBankoAsync(tcKimlikNo);
        }

        public async Task<User?> GetBankoActivePersonelAsync(int bankoId)
        {
            return await _apiService.GetBankoActivePersonelAsync(bankoId);
        }

        public Task<int?> GetBankoIdByConnectionIdAsync(string connectionId)
        {
            _logger.LogWarning("GetBankoIdByConnectionIdAsync - API endpoint henüz implement edilmedi");
            throw new NotImplementedException("API endpoint eklenecek");
        }

        public Task<string?> GetConnectionIdByBankoIdAsync(int bankoId)
        {
            _logger.LogWarning("GetConnectionIdByBankoIdAsync - API endpoint henüz implement edilmedi");
            throw new NotImplementedException("API endpoint eklenecek");
        }

        public Task<Dictionary<int, string>> GetAllActiveBankoConnectionsAsync()
        {
            _logger.LogWarning("GetAllActiveBankoConnectionsAsync - API endpoint henüz implement edilmedi");
            throw new NotImplementedException("API endpoint eklenecek");
        }

        public Task<HubBankoConnection?> GetBankoConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            _logger.LogWarning("GetBankoConnectionByHubConnectionIdAsync - API endpoint henüz implement edilmedi");
            throw new NotImplementedException("API endpoint eklenecek");
        }

        // ═══════════════════════════════════════════════════════
        // TV CONNECTION MANAGEMENT
        // ═══════════════════════════════════════════════════════

        public async Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, string tcKimlikNo)
        {
            return await _apiService.RegisterTvConnectionAsync(tvId, connectionId, tcKimlikNo);
        }

        public async Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId)
        {
            _logger.LogWarning("RegisterTvConnectionAsync tcKimlikNo olmadan çağrıldı - boş string kullanılıyor");
            return await _apiService.RegisterTvConnectionAsync(tvId, connectionId, "");
        }

        public async Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, ConnectionStatus status)
        {
            _logger.LogWarning("RegisterTvConnectionAsync ConnectionStatus ile çağrıldı - boş tcKimlikNo kullanılıyor");
            return await _apiService.RegisterTvConnectionAsync(tvId, connectionId, "");
        }

        public Task<bool> UnregisterTvConnectionAsync(int tvId, string connectionId)
        {
            _logger.LogWarning("UnregisterTvConnectionAsync - API endpoint henüz implement edilmedi");
            throw new NotImplementedException("API endpoint eklenecek");
        }

        public async Task<bool> IsTvInUseByTvUserAsync(int tvId)
        {
            return await _apiService.IsTvInUseByTvUserAsync(tvId);
        }

        public async Task<bool> IsTvConnectedAsync(int tvId)
        {
            return await _apiService.IsTvInUseByTvUserAsync(tvId);
        }

        public Task<int?> GetTvIdByConnectionIdAsync(string connectionId)
        {
            _logger.LogWarning("GetTvIdByConnectionIdAsync - API endpoint henüz implement edilmedi");
            throw new NotImplementedException("API endpoint eklenecek");
        }

        public Task<string?> GetConnectionIdByTvIdAsync(int tvId)
        {
            _logger.LogWarning("GetConnectionIdByTvIdAsync - API endpoint henüz implement edilmedi");
            throw new NotImplementedException("API endpoint eklenecek");
        }

        public Task<Dictionary<int, string>> GetAllActiveConnectionsAsync()
        {
            _logger.LogWarning("GetAllActiveConnectionsAsync - API endpoint henüz implement edilmedi");
            throw new NotImplementedException("API endpoint eklenecek");
        }

        public Task<HubTvConnection?> GetTvConnectionByHubConnectionIdAsync(int hubConnectionId)
        {
            _logger.LogWarning("GetTvConnectionByHubConnectionIdAsync - API endpoint henüz implement edilmedi");
            throw new NotImplementedException("API endpoint eklenecek");
        }

        public async Task<bool> UpdateConnectionStatusAsync(string connectionId, ConnectionStatus status)
        {
            return await _apiService.SetConnectionStatusAsync(connectionId, status.ToString());
        }
    }
}
