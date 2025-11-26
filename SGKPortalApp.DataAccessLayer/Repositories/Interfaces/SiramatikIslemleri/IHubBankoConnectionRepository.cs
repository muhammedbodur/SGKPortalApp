using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    /// <summary>
    /// HubBankoConnection repository interface
    /// </summary>
    public interface IHubBankoConnectionRepository : IGenericRepository<HubBankoConnection>
    {
        /// <summary>
        /// HubConnectionId'ye göre HubBankoConnection getir
        /// </summary>
        Task<HubBankoConnection?> GetByHubConnectionIdAsync(int hubConnectionId);

        /// <summary>
        /// TcKimlikNo'ya göre aktif HubBankoConnection getir
        /// </summary>
        Task<HubBankoConnection?> GetActiveByTcKimlikNoAsync(string tcKimlikNo);

        /// <summary>
        /// BankoId'ye göre aktif HubBankoConnection getir
        /// </summary>
        Task<HubBankoConnection?> GetActiveByBankoIdAsync(int bankoId);

        /// <summary>
        /// HubBankoConnection'ı deaktif et
        /// </summary>
        Task<bool> DeactivateAsync(int hubBankoConnectionId);
    }
}
