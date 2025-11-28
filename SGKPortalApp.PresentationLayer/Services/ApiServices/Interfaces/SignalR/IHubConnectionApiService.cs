using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;

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
        Task<List<HubConnectionResponseDto>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo);

        // Banko Connection Management
        Task<bool> RegisterBankoConnectionAsync(int bankoId, string connectionId, string tcKimlikNo);
        Task<bool> DeactivateBankoConnectionAsync(string tcKimlikNo);
        Task<bool> IsBankoInUseAsync(int bankoId);
        Task<HubBankoConnectionResponseDto?> GetPersonelActiveBankoAsync(string tcKimlikNo);

        // TV Connection Management
        Task<bool> RegisterTvConnectionAsync(int tvId, string connectionId, string tcKimlikNo);
        Task<bool> IsTvInUseByTvUserAsync(int tvId);

        // Connection Status Management
        Task<bool> UpdateConnectionTypeAsync(string connectionId, string connectionType);
        Task<bool> SetConnectionStatusAsync(string connectionId, string status);
        Task<HubConnectionResponseDto?> GetByConnectionIdAsync(string connectionId);

        // New Banko Mode Methods
        Task<bool> CreateBankoConnectionAsync(int hubConnectionId, int bankoId, string tcKimlikNo);
        Task<bool> DeactivateBankoConnectionByHubConnectionIdAsync(int hubConnectionId);
        Task<List<HubConnectionResponseDto>> GetNonBankoConnectionsByTcKimlikNoAsync(string tcKimlikNo);
        Task<HubBankoConnectionResponseDto?> GetBankoConnectionByHubConnectionIdAsync(int hubConnectionId);
        Task<HubTvConnectionResponseDto?> GetTvConnectionByHubConnectionIdAsync(int hubConnectionId);
        Task<UserResponseDto?> GetBankoActivePersonelAsync(int bankoId);
        Task<bool> TransferBankoConnectionAsync(string tcKimlikNo, string connectionId);
    }
}
