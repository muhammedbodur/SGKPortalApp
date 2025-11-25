using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.SignalR
{
    /// <summary>
    /// Hub Connection API Service Interface (Layered Architecture - API üzerinden)
    /// Presentation Layer → API Layer → Business Layer → Data Access Layer
    /// </summary>
    public interface IHubConnectionApiService
    {
        // Connection Management
        Task<bool> CreateOrUpdateUserConnectionAsync(string connectionId, string tcKimlikNo);
        Task<bool> DisconnectAsync(string connectionId);
        Task<List<HubConnection>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo);

        // Banko Connection Management
        Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, string tcKimlikNo);
        Task<bool> DeactivateBankoConnectionAsync(string tcKimlikNo);
        Task<bool> IsBankoInUseAsync(int bankoId);
        Task<HubBankoConnection?> GetPersonelActiveBankoAsync(string tcKimlikNo);
        Task<User?> GetBankoActivePersonelAsync(int bankoId);

        // TV Connection Management
        Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, string tcKimlikNo);
        Task<bool> IsTvInUseByTvUserAsync(int tvId);

        // Connection Status Management
        Task<bool> UpdateConnectionTypeAsync(string connectionId, string connectionType);
        Task<bool> SetConnectionStatusAsync(string connectionId, string status);
        Task<HubConnection?> GetByConnectionIdAsync(string connectionId);
    }
}
