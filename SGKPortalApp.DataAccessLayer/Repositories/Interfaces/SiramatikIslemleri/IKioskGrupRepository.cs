using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKioskGrupRepository : IGenericRepository<KioskGrup>
    {
        // Hizmet binası bazında kiosk gruplarını listeler
        Task<IEnumerable<KioskGrup>> GetByHizmetBinasiAsync(int hizmetBinasiId);

        // Kiosk grup adıyla grubu getirir
        Task<KioskGrup?> GetByAdAsync(string kioskGrupAdi);

        // Kiosk grubunu detaylı getirir
        Task<KioskGrup?> GetWithDetailsAsync(int kioskGrupId);

        // Tüm kiosk gruplarını detaylı listeler
        Task<IEnumerable<KioskGrup>> GetAllWithDetailsAsync();

        // Aktif kiosk gruplarını listeler
        Task<IEnumerable<KioskGrup>> GetActiveAsync();

        // Dropdown için kiosk gruplarını listeler
        Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync();
    }
}