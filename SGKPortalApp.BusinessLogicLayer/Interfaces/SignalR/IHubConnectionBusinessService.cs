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
        Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, string tcKimlikNo);
        Task<bool> IsBankoInUseAsync(int bankoId);
        Task<HubBankoConnection?> GetPersonelActiveBankoAsync(string tcKimlikNo);
        Task<bool> UpdateConnectionTypeAsync(string connectionId, string connectionType);
        Task<bool> SetConnectionStatusAsync(string connectionId, string status);
    }
}
