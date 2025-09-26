using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
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
    }
}