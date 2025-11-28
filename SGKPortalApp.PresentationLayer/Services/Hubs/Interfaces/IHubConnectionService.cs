using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces
{
    /// <summary>
    /// Hub bağlantı yönetimi için servis interface
    /// </summary>
    public interface IHubConnectionService
    {
        /// <summary>
        /// Authenticated kullanıcı için HubConnection oluştur veya güncelle
        /// </summary>
        Task<bool> CreateOrUpdateUserConnectionAsync(string connectionId, string tcKimlikNo);

        /// <summary>
        /// TV bağlantısını kaydet veya güncelle
        /// </summary>
        Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, ConnectionStatus status);

        /// <summary>
        /// TV bağlantısını kaldır
        /// </summary>
        Task<bool> UnregisterTvConnectionAsync(int tvId, string connectionId);

        /// <summary>
        /// TV'nin bağlantı durumunu kontrol et
        /// </summary>
        Task<bool> IsTvConnectedAsync(int tvId);

        /// <summary>
        /// ConnectionId'ye göre TV ID'sini al
        /// </summary>
        Task<int?> GetTvIdByConnectionIdAsync(string connectionId);

        /// <summary>
        /// TV'nin connection ID'sini al
        /// </summary>
        Task<string?> GetConnectionIdByTvIdAsync(int tvId);

        /// <summary>
        /// Tüm aktif bağlantıları al
        /// </summary>
        Task<Dictionary<int, string>> GetAllActiveConnectionsAsync();

        /// <summary>
        /// Bağlantı durumunu güncelle
        /// </summary>
        Task<bool> UpdateConnectionStatusAsync(string connectionId, ConnectionStatus status);

        // ═══════════════════════════════════════════════════════
        // BANKO CONNECTION MANAGEMENT
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Banko bağlantısını kaydet veya güncelle
        /// </summary>
        Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, ConnectionStatus status);

        /// <summary>
        /// Banko bağlantısını kaldır
        /// </summary>
        Task<bool> UnregisterBankoConnectionAsync(int bankoId, string connectionId);

        /// <summary>
        /// Banko'nun bağlantı durumunu kontrol et
        /// </summary>
        Task<bool> IsBankoConnectedAsync(int bankoId);

        /// <summary>
        /// ConnectionId'ye göre Banko ID'sini al
        /// </summary>
        Task<int?> GetBankoIdByConnectionIdAsync(string connectionId);

        /// <summary>
        /// Banko'nun connection ID'sini al
        /// </summary>
        Task<string?> GetConnectionIdByBankoIdAsync(int bankoId);

        /// <summary>
        /// Tüm aktif banko bağlantılarını al
        /// </summary>
        Task<Dictionary<int, string>> GetAllActiveBankoConnectionsAsync();

        // ═══════════════════════════════════════════════════════
        // MULTI-CONNECTION MANAGEMENT (Yeni Yapı)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Kullanıcının tüm aktif bağlantılarını getir (TcKimlikNo bazında)
        /// </summary>
        Task<List<HubConnectionResponseDto>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo);

        /// <summary>
        /// Banko başka personel tarafından kullanılıyor mu?
        /// </summary>
        Task<bool> IsBankoInUseAsync(int bankoId);

        /// <summary>
        /// Personelin aktif banko oturumunu getir
        /// </summary>
        Task<HubBankoConnectionResponseDto?> GetPersonelActiveBankoAsync(string tcKimlikNo);

        /// <summary>
        /// Banko oturumu oluştur (Fiziksel bankoya geçiş)
        /// </summary>
        Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, string tcKimlikNo);

        /// <summary>
        /// Banko oturumunu kapat (Banko modundan çıkış)
        /// </summary>
        Task<bool> DeactivateBankoConnectionAsync(string tcKimlikNo);

        /// <summary>
        /// Bağlantı tipini güncelle (MainLayout, TvDisplay, BankoMode)
        /// </summary>
        Task<bool> UpdateConnectionTypeAsync(string connectionId, string connectionType);

        /// <summary>
        /// Bağlantı durumunu güncelle (online, offline, reconnecting)
        /// </summary>
        Task<bool> SetConnectionStatusAsync(string connectionId, string status);

        /// <summary>
        /// ConnectionId ile HubConnection getir
        /// </summary>
        Task<HubConnectionResponseDto?> GetByConnectionIdAsync(string connectionId);

        /// <summary>
        /// Bağlantıyı tamamen kapat ve sil
        /// </summary>
        Task<bool> DisconnectAsync(string connectionId);

        /// <summary>
        /// TV bağlantısını kaydet (Yeni yapı - birden fazla kullanıcı)
        /// </summary>
        Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId);

        /// <summary>
        /// TV başka bir TV User tarafından kullanılıyor mu?
        /// </summary>
        Task<bool> IsTvInUseByTvUserAsync(int tvId);




        /// <summary>
        /// Kullanıcı için yeni bağlantı oluştur (ConnectionType ile)
        /// </summary>
        Task<bool> RegisterUserConnectionAsync(string connectionId, string tcKimlikNo, string connectionType);

        /// <summary>
        /// Banko modu için HubBankoConnection oluştur
        /// </summary>
        Task<bool> CreateBankoConnectionAsync(int hubConnectionId, int bankoId, string tcKimlikNo);

        /// <summary>
        /// HubConnectionId'ye göre HubBankoConnection'ı deaktif et ve User tablosunu temizle
        /// </summary>
        Task<bool> DeactivateBankoConnectionByHubConnectionIdAsync(int hubConnectionId);

        /// <summary>
        /// HubBankoConnection olmayan (normal) bağlantıları getir
        /// </summary>
        Task<List<HubConnectionResponseDto>> GetNonBankoConnectionsByTcKimlikNoAsync(string tcKimlikNo);

        /// <summary>
        /// HubConnectionId ile HubBankoConnection getir
        /// </summary>
        Task<HubBankoConnectionResponseDto?> GetBankoConnectionByHubConnectionIdAsync(int hubConnectionId);

        /// <summary>
        /// HubConnectionId ile HubTvConnection getir
        /// </summary>
        Task<HubTvConnectionResponseDto?> GetTvConnectionByHubConnectionIdAsync(int hubConnectionId);

        /// <summary>
        /// Bankodaki aktif personeli getir
        /// </summary>
        Task<UserResponseDto?> GetBankoActivePersonelAsync(int bankoId);

        /// <summary>
        /// Banko bağlantısını transfer et
        /// </summary>
        Task<bool> TransferBankoConnectionAsync(string tcKimlikNo, string connectionId);

        // ═══════════════════════════════════════════════════════
        // TV MODE METHODS (mirroring Banko pattern)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// TV modu için HubTvConnection oluştur
        /// </summary>
        Task<bool> CreateTvConnectionAsync(int hubConnectionId, int tvId, string tcKimlikNo);

        /// <summary>
        /// HubConnectionId'ye göre HubTvConnection'ı deaktif et
        /// </summary>
        Task<bool> DeactivateTvConnectionByHubConnectionIdAsync(int hubConnectionId);

        /// <summary>
        /// HubTvConnection olmayan bağlantıları getir
        /// </summary>
        Task<List<HubConnectionResponseDto>> GetNonTvConnectionsByTcKimlikNoAsync(string tcKimlikNo);

        /// <summary>
        /// Personelin aktif TV bağlantısını getir
        /// </summary>
        Task<HubTvConnectionResponseDto?> GetActiveTvByTcKimlikNoAsync(string tcKimlikNo);

        /// <summary>
        /// TV'deki aktif kullanıcıyı getir
        /// </summary>
        Task<UserResponseDto?> GetTvActiveUserAsync(int tvId);

        /// <summary>
        /// TV bağlantısını transfer et
        /// </summary>
        Task<bool> TransferTvConnectionAsync(string tcKimlikNo, string connectionId);
    }
}
