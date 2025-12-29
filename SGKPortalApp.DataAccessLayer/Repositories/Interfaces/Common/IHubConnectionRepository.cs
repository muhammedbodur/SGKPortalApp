using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IHubConnectionRepository : IGenericRepository<HubConnection>
    {
        // Kullanıcıya ait bağlantıları listeler
        Task<IEnumerable<HubConnection>> GetByUserAsync(string tcKimlikNo);

        // Bağlantı ID'sine göre bağlantıyı getirir
        Task<HubConnection?> GetByConnectionIdAsync(string connectionId);

        // Aktif bağlantıları listeler
        Task<IEnumerable<HubConnection>> GetActiveConnectionsAsync();

        // Bağlantıyı detaylı getirir
        Task<HubConnection?> GetWithDetailsAsync(int hubConnectionId);

        // Tüm bağlantıları detaylı listeler
        Task<IEnumerable<HubConnection>> GetAllWithDetailsAsync();

        // Kullanıcının aktif bağlantılarını listeler (BankoMode için)
        Task<IEnumerable<HubConnection>> GetActiveConnectionsByTcKimlikNoAsync(string tcKimlikNo);

        // Cleanup işlemleri (Background Service için)
        Task<IEnumerable<HubConnection>> GetStaleConnectionsAsync(System.DateTime timeoutThreshold);
        Task<int> DeactivateStaleConnectionsAsync(System.DateTime timeoutThreshold);
        Task<IEnumerable<string>> GetActiveSessionIdsAsync();
    }
}
