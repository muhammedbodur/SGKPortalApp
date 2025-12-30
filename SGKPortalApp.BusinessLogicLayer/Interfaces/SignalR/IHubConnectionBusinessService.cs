using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR
{
    /// <summary>
    /// Hub Connection Business Service Interface
    /// Business Layer'da SignalR bağlantı yönetimi
    /// </summary>
    public interface IHubConnectionBusinessService
    {
        Task<bool> CreateOrUpdateConnectionAsync(string connectionId, string tcKimlikNo);
        Task<bool> DisconnectAsync(string connectionId);
        Task<IEnumerable<HubConnection>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo);
        Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, string tcKimlikNo);
        Task<bool> DeactivateBankoConnectionAsync(string tcKimlikNo);
        Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId);
        Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, string tcKimlikNo);
        Task<bool> IsBankoInUseAsync(int bankoId);
        Task<HubBankoConnection?> GetPersonelActiveBankoAsync(string tcKimlikNo);
        Task<bool> UpdateConnectionTypeAsync(string connectionId, string connectionType);
        Task<bool> SetConnectionStatusAsync(string connectionId, string status);

        // New Banko Mode Methods
        Task<bool> CreateBankoConnectionAsync(int hubConnectionId, int bankoId, string tcKimlikNo);
        Task<bool> DeactivateBankoConnectionByHubConnectionIdAsync(int hubConnectionId);
        Task<List<HubConnection>> GetNonBankoConnectionsByTcKimlikNoAsync(string tcKimlikNo);
        Task<HubConnection?> GetByConnectionIdAsync(string connectionId);
        Task<HubBankoConnection?> GetBankoConnectionByHubConnectionIdAsync(int hubConnectionId);
        Task<HubTvConnection?> GetTvConnectionByHubConnectionIdAsync(int hubConnectionId);
        Task<User?> GetBankoActivePersonelAsync(int bankoId);
        Task<bool> TransferBankoConnectionAsync(string tcKimlikNo, string newConnectionId);

        // TV Mode Methods (mirroring Banko pattern)
        Task<bool> CreateTvConnectionAsync(int hubConnectionId, int tvId, string tcKimlikNo);
        Task<bool> DeactivateTvConnectionByHubConnectionIdAsync(int hubConnectionId);
        Task<HubTvConnection?> GetActiveTvByTcKimlikNoAsync(string tcKimlikNo);
        Task<User?> GetTvActiveUserAsync(int tvId);
        Task<bool> TransferTvConnectionAsync(string tcKimlikNo, string newConnectionId);
        Task<bool> IsTvInUseByTvUserAsync(int tvId);
        Task<bool> IsTvInUseByOtherTvUserAsync(int tvId, string currentTcKimlikNo);
        Task<List<HubConnection>> GetNonTvConnectionsByTcKimlikNoAsync(string tcKimlikNo);

        // Cleanup Methods (Background Service için API endpoint'ler)
        /// <summary>
        /// Uygulama başlangıcında tüm online connection'ları offline yapar (restart cleanup)
        /// </summary>
        Task<int> CleanupAllOnStartupAsync();

        /// <summary>
        /// Stale (eski) connection'ları temizler (belirli süre aktivite olmayanlar)
        /// </summary>
        /// <param name="staleThresholdMinutes">Stale kabul edilme süresi (dakika)</param>
        Task<int> CleanupStaleConnectionsAsync(int staleThresholdMinutes);

        /// <summary>
        /// Orphan HubBankoConnection kayıtlarını temizler
        /// HubConnection offline/silinmiş ama HubBankoConnection hala aktif olanları bulur ve temizler
        /// </summary>
        Task<(int count, List<string> cleanedTcKimlikNoList)> CleanupOrphanBankoConnectionsAsync();

        /// <summary>
        /// Orphan HubTvConnection kayıtlarını temizler
        /// HubConnection offline/silinmiş ama HubTvConnection hala aktif olanları bulur ve temizler
        /// </summary>
        Task<int> CleanupOrphanTvConnectionsAsync();
    }
}

