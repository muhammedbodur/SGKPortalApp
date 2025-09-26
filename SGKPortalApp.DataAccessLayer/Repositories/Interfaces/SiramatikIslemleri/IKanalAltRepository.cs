using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKanalAltRepository : IGenericRepository<KanalAlt>
    {
        // Kanal bazında alt kanalları listeler
        Task<IEnumerable<KanalAlt>> GetByKanalAsync(int kanalId);

        // Alt kanal adıyla alt kanalı getirir
        Task<KanalAlt?> GetByAdAsync(string kanalAltAdi);

        // Alt kanalı detaylı getirir
        Task<KanalAlt?> GetWithDetailsAsync(int kanalAltId);

        // Tüm alt kanalları detaylı listeler
        Task<IEnumerable<KanalAlt>> GetAllWithDetailsAsync();

        // Aktif alt kanalları listeler
        Task<IEnumerable<KanalAlt>> GetActiveAsync();

        // Dropdown için alt kanalları listeler
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
    }
}