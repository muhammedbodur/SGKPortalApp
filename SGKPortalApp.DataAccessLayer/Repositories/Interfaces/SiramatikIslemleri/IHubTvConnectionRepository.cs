using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IHubTvConnectionRepository : IGenericRepository<HubTvConnection>
    {
        // TV'ye ait bağlantıları listeler
        Task<IEnumerable<HubTvConnection>> GetByTvAsync(int tvId);

        // Bağlantı ID'sine göre TV bağlantısını getirir
        Task<HubTvConnection?> GetByConnectionIdAsync(string connectionId);

        // Aktif TV bağlantılarını listeler
        Task<IEnumerable<HubTvConnection>> GetActiveConnectionsAsync();

        // TV bağlantısını detaylı getirir
        Task<HubTvConnection?> GetWithDetailsAsync(int hubTvConnectionId);

        // Tüm TV bağlantılarını detaylı listeler
        Task<IEnumerable<HubTvConnection>> GetAllWithDetailsAsync();
    }
}