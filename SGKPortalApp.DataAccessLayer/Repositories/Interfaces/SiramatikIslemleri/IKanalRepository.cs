using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKanalRepository : IGenericRepository<Kanal>
    {
        // Kanal adıyla kanalı getirir
        Task<Kanal?> GetByKanalAdiAsync(string kanalAdi);

        // Kanalı alt kanallarıyla getirir
        Task<Kanal?> GetWithKanalAltAsync(int kanalId);

        // Tüm kanalları alt kanallarıyla listeler
        Task<IEnumerable<Kanal>> GetAllWithKanalAltAsync();

        // Aktif kanalları listeler
        Task<IEnumerable<Kanal>> GetActiveAsync();

        // Tüm kanalları listeler
        Task<IEnumerable<Kanal>> GetAllAsync();

        // Dropdown için kanalları listeler
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();

        // Kanalın alt kanal sayısını getirir
        Task<int> GetKanalAltCountAsync(int kanalId);
    }
}